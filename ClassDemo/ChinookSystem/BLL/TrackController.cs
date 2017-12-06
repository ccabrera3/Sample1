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
    [DataObject]
    public class TrackController
    {
        [DataObjectMethod(DataObjectMethodType.Select,false)]
        public List<TrackList> List_TracksForPlaylistSelection(string tracksby, int argid)
        {
            using (var context = new ChinookContext())
            {
                IEnumerable<TrackList> results = null;

               //code to go here
               switch(tracksby)
                {
                    case "Artist":
                        {
                            results = from x in context.Tracks
                                      orderby x.Name
                                      where x.Album.ArtistId == argid
                                      select new TrackList
                                      {
                                          TrackID = x.TrackId,
                                          Name = x.Name,
                                          Title = x.Album.Title,
                                          MediaName = x.MediaType.Name,
                                          GenreName = x.Genre.Name,
                                          Composer = x.Composer,
                                          Milliseconds = x.Milliseconds,
                                          Bytes = x.Bytes,
                                          UnitPrice = x.UnitPrice
                                      };
                            break;
                        }

                    case "MediaType":
                        {
                            results = from x in context.Tracks
                                      orderby x.Name
                                      where x.Album.ArtistId == argid
                                      select new TrackList
                                      {
                                          TrackID = x.TrackId,
                                          Name = x.Name,
                                          Title = x.Album.Title,
                                          MediaName = x.MediaType.Name,
                                          GenreName = x.Genre.Name,
                                          Composer = x.Composer,
                                          Milliseconds = x.Milliseconds,
                                          Bytes = x.Bytes,
                                          UnitPrice = x.UnitPrice
                                      };
                            break;
               
                        }
                    case "Genre":
                        {
                            results = from x in context.Tracks
                                      orderby x.Name
                                      where x.Album.ArtistId == argid
                                      select new TrackList
                                      {
                                          TrackID = x.TrackId,
                                          Name = x.Name,
                                          Title = x.Album.Title,
                                          MediaName = x.MediaType.Name,
                                          GenreName = x.Genre.Name,
                                          Composer = x.Composer,
                                          Milliseconds = x.Milliseconds,
                                          Bytes = x.Bytes,
                                          UnitPrice = x.UnitPrice
                                      };
                            break;
                 
                        }

                    default:
                        {
                            results = from x in context.Tracks
                                      orderby x.Name
                                      where x.Album.ArtistId == argid
                                      select new TrackList
                                      {
                                          TrackID = x.TrackId,
                                          Name = x.Name,
                                          Title = x.Album.Title,
                                          MediaName = x.MediaType.Name,
                                          GenreName = x.Genre.Name,
                                          Composer = x.Composer,
                                          Milliseconds = x.Milliseconds,
                                          Bytes = x.Bytes,
                                          UnitPrice = x.UnitPrice
                                      };
                            break;
                        }

                }

                return results.ToList();
            }
        }//eom
         //where tracksby.Equals("Artist") ? x.Album.AritistId == argid :
         //tracksby.Equals("MediaType") ? x.MediaTypeId == argid :
         //trackby.Equals("Genre) ? x.GenreId == argid:
         //x.AlbumId == argid

        [DataObjectMethod(DataObjectMethodType.Select,false)]
        public List<GenreAlbumReport> GenreAlbumReport_Get()
        {
            using (var context = new ChinookContext())
            {
                var results = from x in context.Tracks
                              select new GenreAlbumReport
                              {
                                  GenreName = x.Genre.Name,
                                  AlbumTitle = x.Album.Title,
                                  TrackName = x.Name,
                                  Milliseconds = x.Milliseconds,
                                  Bytes = x.Bytes,
                                  UnitPrice = x.UnitPrice
                              };

                return results.ToList();
            }
        }


    }//eoc
}
