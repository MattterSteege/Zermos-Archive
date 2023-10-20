using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models.Zermos;

namespace Zermos_Web.Controllers
{
    public class NotitiesController : BaseController
    {
        public NotitiesController(Users user, ILogger<BaseController> logger) : base(user, logger) { }

        // JSON Parsing Helper
        private NotitiesModel ParseNotitiesModel()
        {
            return JsonConvert.DeserializeObject<NotitiesModel>(ZermosUser.notities ?? "{}");
        }

        #region Notitie boeken

        [Route("/Notities")]
        [Route("/Notities/Boeken")]
        public IActionResult Index()
        {
            var notes = ParseNotitiesModel();
            if (notes.NotitieBoeken == null)
            {
                notes.NotitieBoeken = new List<NotitieBoek>();
                ZermosUser = new user {notities = JsonConvert.SerializeObject(notes)};
            }

            return PartialView(notes);
        }

        #endregion

        #region Notitie boek

        [HttpGet]
        [Route("/Notities/{notitieboekId}")]
        public IActionResult GetNoteBook(string notitieboekId)
        {
            var model = ParseNotitiesModel();
            var notitieBoek = model.NotitieBoeken.FirstOrDefault(x => x.Id == notitieboekId);
            return PartialView(notitieBoek);
        }

        [HttpPost]
        [Route("/Notities/")]
        public IActionResult AddNoteBook([FromBody] NotitieBoek notitieBoek)
        {
            var model = ParseNotitiesModel();
            notitieBoek.Id = Guid.NewGuid().ToString();
            notitieBoek.ranking = model.NotitieBoeken.Count + 1;
            notitieBoek.Notities = new List<Notitie>();
            model.NotitieBoeken.Add(notitieBoek);
            ZermosUser = new user {notities = JsonConvert.SerializeObject(model)};
            return Ok();
        }

        #endregion

        #region Notities

        [HttpGet]
        [Route("/Notities/{notitieboekId}/{notitieId}")]
        public IActionResult GetNotitie(string notitieboekId, string notitieId)
        {
            var model = ParseNotitiesModel();
            var notitie = model.NotitieBoeken.FirstOrDefault(x => x.Id == notitieboekId)?.Notities.FirstOrDefault(x => x.Id == notitieId);
            return PartialView(notitie);
        }

        [HttpPost]
        [Route("/Notities/{notitieboekId}")]
        public IActionResult AddNotitie(string notitieboekId, [FromBody] Notitie notitie)
        {
            var model = ParseNotitiesModel();
            notitie.Id = Guid.NewGuid().ToString();
            var notitieBoek = model.NotitieBoeken.FirstOrDefault(x => x.Id == notitieboekId);
            if (notitieBoek != null)
            {
                notitie.ranking = notitieBoek.Notities.Count + 1;
                notitie.Paragraphs = new List<Paragraph>();
                notitieBoek.Notities.Add(notitie);
                ZermosUser = new user {notities = JsonConvert.SerializeObject(model)};
                return Ok();
            }

            return NotFound();
        }

        #endregion

        #region Paragrafen

        [HttpGet]
        [Route("/Notities/{notitieboekId}/{notitieId}/{paragraphId}")]
        public IActionResult GetParagraph(string notitieboekId, string notitieId, string paragraphId)
        {
            var model = ParseNotitiesModel();
            var notitieBoek = model.NotitieBoeken.FirstOrDefault(x => x.Id == notitieboekId);
            var notitie = notitieBoek?.Notities.FirstOrDefault(x => x.Id == notitieId);
            var paragraph = notitie?.Paragraphs.FirstOrDefault(x => x.Id == paragraphId);
            return PartialView(paragraph);
        }
        
        [HttpPut]
        [Route("/Notities/{notitieboekId}/{notitieId}/{paragraphId}")]
        public IActionResult UpdateParagraph(string notitieboekId, string notitieId, string paragraphId, [FromBody] Paragraph paragraph)
        {
            var model = ParseNotitiesModel();
            var notitieBoek = model.NotitieBoeken.FirstOrDefault(x => x.Id == notitieboekId);
            var notitie = notitieBoek?.Notities.FirstOrDefault(x => x.Id == notitieId);
            var oldParagraph = notitie?.Paragraphs.FirstOrDefault(x => x.Id == paragraphId);
            if (oldParagraph != null)
            {
                // oldParagraph.ranking = paragraph.ranking != 0 ? paragraph.ranking : oldParagraph.ranking;
                // oldParagraph.Inhoud = paragraph.Inhoud ?? oldParagraph.Inhoud;
                // oldParagraph.Naam = paragraph.Naam ?? oldParagraph.Naam;
                
                if (paragraph.Naam != null && paragraph.Naam != oldParagraph.Naam)
                    oldParagraph.Naam = paragraph.Naam;
                
                if (paragraph.Inhoud != null && paragraph.Inhoud != oldParagraph.Inhoud)
                    oldParagraph.Inhoud = paragraph.Inhoud;

                if (paragraph.ranking != 0 && paragraph.ranking != oldParagraph.ranking)
                {
                    int rankingDifference = paragraph.ranking - oldParagraph.ranking;
                    
                    // If the ranking is higher than the old ranking, we need to move the other paragraphs down
                    if (rankingDifference > 0)
                    {
                        // Get all the paragraphs that need to be moved down
                        var paragraphsToMoveDown = notitie.Paragraphs.Where(x => x.ranking > oldParagraph.ranking && x.ranking <= paragraph.ranking).ToList();
                        foreach (var paragraphToMoveDown in paragraphsToMoveDown)
                        {
                            paragraphToMoveDown.ranking--;
                        }
                    }
                    
                    // If the ranking is lower than the old ranking, we need to move the other paragraphs up
                    if (rankingDifference < 0)
                    {
                        // Get all the paragraphs that need to be moved up
                        var paragraphsToMoveUp = notitie.Paragraphs.Where(x => x.ranking < oldParagraph.ranking && x.ranking >= paragraph.ranking).ToList();
                        foreach (var paragraphToMoveUp in paragraphsToMoveUp)
                        {
                            paragraphToMoveUp.ranking++;
                        }
                    }
                    
                    oldParagraph.ranking = paragraph.ranking;
                    
                    //sort the list
                    notitie.Paragraphs = notitie.Paragraphs.OrderBy(x => x.ranking).ToList();
                }
                
                
                ZermosUser = new user {notities = JsonConvert.SerializeObject(model)};
                return Ok();
            }

            return NotFound();
        }
        
        [HttpDelete]
        [Route("/Notities/{notitieboekId}/{notitieId}/{paragraphId}")]
        public IActionResult DeleteParagraph(string notitieboekId, string notitieId, string paragraphId)
        {
            var model = ParseNotitiesModel();
            var notitieBoek = model.NotitieBoeken.FirstOrDefault(x => x.Id == notitieboekId);
            var notitie = notitieBoek?.Notities.FirstOrDefault(x => x.Id == notitieId);
            var paragraph = notitie?.Paragraphs.FirstOrDefault(x => x.Id == paragraphId);
            if (paragraph != null)
            {
                notitie.Paragraphs.Remove(paragraph);
                ZermosUser = new user {notities = JsonConvert.SerializeObject(model)};
                return Ok();
            }

            return NotFound();
        }

        [HttpPost]
        [Route("/Notities/{notitieboekId}/{notitieId}")]
        public IActionResult AddParagraph(string notitieboekId, string notitieId, [FromBody] Paragraph paragraph)
        {
            var model = ParseNotitiesModel();
            var notitieBoek = model.NotitieBoeken.FirstOrDefault(x => x.Id == notitieboekId);
            if (notitieBoek != null)
            {
                paragraph.Id = Guid.NewGuid().ToString();
                paragraph.ranking = notitieBoek.Notities.FirstOrDefault(x => x.Id == notitieId)?.Paragraphs.Count + 1 ?? 1;
                notitieBoek.Notities.FirstOrDefault(x => x.Id == notitieId)?.Paragraphs.Add(paragraph);
                ZermosUser = new user {notities = JsonConvert.SerializeObject(model)};
                return Ok();
            }

            return NotFound();
        }

        #endregion
    }
}