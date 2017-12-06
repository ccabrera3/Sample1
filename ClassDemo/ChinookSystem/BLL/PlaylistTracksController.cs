using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Chinook.Data.Entities;
using Chinook.Data.DTOs;
using Chinook.Data.POCOs;
using ChinookSystem.DAL;
using System.ComponentModel;
#endregion

namespace ChinookSystem.BLL
{
    public class PlaylistTracksController
    {
        public List<UserPlaylistTrack> List_TracksForPlaylist(
            string playlistname, string username)
        {
            using (var context = new ChinookContext())
            {

                //code to go here
                //what would happen if there is no match for the incoming parameter values
                //we need to ensure that results has a valid value
                // these values will be an IEnumerable<T> collection or 
                //it should be null
                //To ensure that results does end up with a valid value
                //use the .FirstOrDefault()
                var results = (from x in context.Playlists
                              where x.UserName.Equals(username)
                              && x.Name.Equals(playlistname)
                              select x).FirstOrDefault();

                var theTracks = from x in context.PlaylistTracks
                                where x.PlaylistId.Equals(results.PlaylistId)
                                orderby x.TrackNumber
                                select new UserPlaylistTrack //one of the poco's we created
                                {
                                    TrackID = x.TrackId,
                                    TrackNumber = x.TrackNumber,
                                    TrackName = x.Track.Name,
                                    Milliseconds = x.Track.Milliseconds,
                                    UnitPrice = x.Track.UnitPrice
                                };

                return theTracks.ToList();


                  

               
            }
        }//eom
        public List<UserPlaylistTrack> Add_TrackToPLaylist(string playlistname, string username, int trackid)
        {
            using (var context = new ChinookContext())
            {
                //code to go here
                //PART ONE : HANDLE PLAYLIST RECORD
                //query to get playlistid
                var exists = (from x in context.Playlists
                              where x.UserName.Equals(username) && x.Name.Equals(playlistname)
                              select x).FirstOrDefault();

                //initialize the tracknumber for the track going into PlaylistTracks
                int tracknumber = 0;
                //i will need to create an instance of PlaylistTrack
                PlaylistTrack newtrack = null;
                //determine if this is an addition to an existing list or
                // if a new list needs to be created
                if (exists == null)
                {
                    //this is a new playlist
                    //create the playlist
                    exists = new Playlist();
                    exists.Name = playlistname;
                    exists.UserName = username;
                    exists = context.Playlists.Add(exists);
                    tracknumber = 1;
                }
                else
                {
                    //the playlist already exists
                    //i need to know the number of tracks currently on the list
                    //track number will be = count + 1
                    tracknumber = exists.PlaylistTracks.Count() + 1;

                    //in our example, trakc exists only once on each playlist
                    newtrack = exists.PlaylistTracks.SingleOrDefault(x => x.TrackId == trackid);
                    //this will be null if the track is NOT on the playlist tracks
                    if(newtrack != null)
                    {
                        throw new Exception("Playlist already has requested track");
                    }

                }

                //PART TWO: HANDLE THE TRACK FOR PLAYLIST TRACK
                //use navigation to .Add the new track to PlaylistTrack
                newtrack = new PlaylistTrack();
                newtrack.TrackId = trackid;
                newtrack.TrackNumber = tracknumber;

                //NOTE: the pkey for PlaylistId may not yet exist
                //using navigation one can let HashSet handle the PlaylistId pkey
                exists.PlaylistTracks.Add(newtrack);

                //physically commit your woirk to the database\
                context.SaveChanges();
                return List_TracksForPlaylist(playlistname, username);

            }
        }//eom
        public void MoveTrack(string username, string playlistname, int trackid, int tracknumber, string direction)
        {
            using (var context = new ChinookContext())
            {
                //code to go here 
                var exists = (from x in context.Playlists
                              where x.UserName.Equals(username) && x.Name.Equals(playlistname)
                              select x).FirstOrDefault();
                if(exists == null)
                {
                    throw new Exception("Playlist has been removed");
                }
                else
                {
                    //limit your search to the particular playlist
                    PlaylistTrack movetrack = (from x in exists.PlaylistTracks
                                               where x.TrackId == trackid
                                               select x).FirstOrDefault();

                    if(movetrack == null)
                    {
                        throw new Exception("Playlist track has been removed");
                    }
                    else
                    {
                        PlaylistTrack othertrack = null;
                        if (direction.Equals("up"))
                        {
                            if(movetrack.TrackNumber == 1)
                            {
                                throw new Exception("Playlist track cannot be moved");
                            }
                            else
                            {
                                //get the other track
                                othertrack = (from x in exists.PlaylistTracks
                                                           where x.TrackNumber == movetrack.TrackNumber - 1
                                                           select x).FirstOrDefault();
                                if(othertrack == null)
                                {
                                    throw new Exception("Playlist track cannot be moved UP");
                                }
                                else
                                {
                                    //at this point you can exchange track numbers
                                    movetrack.TrackNumber -= 1;
                                    othertrack.TrackNumber += 1;
                                }
                            }
                        }
                        else //for DOWN
                        {
                            if (movetrack.TrackNumber >= exists.PlaylistTracks.Count)
                            {
                                throw new Exception("Playlist track cannot be moved down");
                            }
                            else
                            {
                                //get the other track
                                othertrack = (from x in exists.PlaylistTracks
                                              where x.TrackNumber == movetrack.TrackNumber + 1
                                              select x).FirstOrDefault();
                                if (othertrack == null)
                                {
                                    throw new Exception("Playlist track cannot be moved DOWN");
                                }
                                else
                                {
                                    //at this point you can exchange track numbers
                                    movetrack.TrackNumber += 1;
                                    othertrack.TrackNumber -= 1;
                                }
                            }
                        } // END OF UP/DOWN
                        //stage changes for the SaveChanges()
                        //indicate only the field that needs to be updated
                        context.Entry(movetrack).Property(y => y.TrackNumber).IsModified = true;
                        context.Entry(othertrack).Property(y => y.TrackNumber).IsModified = true;
                        context.SaveChanges();


                    }
                }

            }
        }//eom


        public List<UserPlaylistTrack> DeleteTracks(string username, string playlistname, List<int> trackstodelete)
        {
            using (var context = new ChinookContext())
            {
                //code to go here
                Playlist exists = (from x in context.Playlists
                                   where x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)
                                   && x.Name.Equals(playlistname, StringComparison.OrdinalIgnoreCase)
                                   select x).FirstOrDefault();
                if(exists == null)
                {
                    throw new Exception("Playlist has been removed from our site.");
                }
                else
                {
                    //the tracks that are to be kept
                    var trackskept = exists.PlaylistTracks.Where(tr => !trackstodelete.Any(tod => tod == tr.TrackId)).Select(tr => tr);
                    //remove the tracks in the trackstodelete list

                    PlaylistTrack item = null;
                    foreach(var deletetrack in trackstodelete)
                    {
                        item = exists.PlaylistTracks
                            .Where(dx => dx.TrackId == deletetrack)
                            .FirstOrDefault();
                        if(item != null)
                        {
                            exists.PlaylistTracks.Remove(item);
                        }
                    }

                    //renumber the kept tracks so that the tracknumber 
                    //is sequential as expected by akk other operations
                    // In our database there is NO holes in the numeric sequence.
                    int number = 1;
                    foreach(var trackkept in trackskept)
                    {
                        trackkept.TrackNumber = number;
                        context.Entry(trackkept).Property(y => y.TrackNumber).IsModified = true;
                        number++;
                    }
                    context.SaveChanges();
                    return List_TracksForPlaylist(playlistname, username);
                }




            }
        }//eom
    }
}
