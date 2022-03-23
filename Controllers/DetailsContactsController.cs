﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AnimAlerte.Models;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using AnimAlerte.ViewModels;
using AnimAlerte.Controllers;
using System.IO;

namespace AnimAlerte.Controllers
{

    public class DetailsContactsController : Controller
    {
        private readonly AnimAlerteContext _context;
        private readonly ISession session;
        public static string usersession;
        public static int admin = 0;

        public DetailsContactsController(AnimAlerteContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            this.session = accessor.HttpContext.Session;
        }

        // GET: DetailsContacts
        public IActionResult Index()
        {
            //on recupere la liste des contacts favoris de l'utilisateur connecté
            var listeContactsFavoris = _context.DetailsContacts.Where(a => a.NomUtilisateurCreateur == UtilisateursController.usersession).ToList();
            //on recuepere la liste de tous les users
            var listeUsers = _context.Utilisateurs.ToList();

            List<DetailsContact> detailsContactsFavoris = new List<DetailsContact>();
            if (listeContactsFavoris != null && listeUsers!=null)
            {
                foreach (var contact in listeContactsFavoris)
                {
                    foreach (var user in listeUsers)
                    {
                        if(contact.NomUtilisateurFavoris==user.NomUtilisateur && user.UtilisateurActive == 1)
                        {
                            detailsContactsFavoris.Add(contact);
                        }
                    }
                }
                return View(detailsContactsFavoris);
            }

            return View();
        }
        
        // GET: DetailsContacts
        public IActionResult Search(string utilisateursSearch)
        {
            var animAlerteContext = _context.Utilisateurs
                .Where((a => a.NomUtilisateur.Contains(utilisateursSearch) || a.Nom.Contains(utilisateursSearch) || a.Prenom.Contains(utilisateursSearch)));
            return View(animAlerteContext.ToList());
                       
        }

        // GET: DetailsContacts/Details/5
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detailsContact = _context.DetailsContacts.SingleOrDefault(c => c.NomUtilisateurFavoris == id);

            return View(detailsContact);
        }

        // GET: DetailsContacts/Create
        [HttpGet]
        public IActionResult Create()
        {
            //var animAlerteContext = _context.DetailsContacts.Where(a => a.NomUtilisateurCreateur == UtilisateursController.usersession).Include(d => d.NomUtilisateurCreateurNavigation).Include(d => d.NomUtilisateurFavorisNavigation);
            //return View();
            ViewData["NomUtilisateurCreateur"] = new SelectList(_context.Utilisateurs, "NomUtilisateur", "NomUtilisateur");
            ViewData["NomUtilisateurFavoris"] = new SelectList(_context.Utilisateurs, "NomUtilisateur", "NomUtilisateur");
            return View();
        }

        // POST: DetailsContacts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomUtilisateurCreateur,NomUtilisateurFavoris,DateAjout")] DetailsContact detailsContact)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detailsContact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NomUtilisateurCreateur"] = new SelectList(_context.Utilisateurs, "NomUtilisateur", "NomUtilisateur", detailsContact.NomUtilisateurCreateur);
            ViewData["NomUtilisateurFavoris"] = new SelectList(_context.Utilisateurs, "NomUtilisateur", "NomUtilisateur", detailsContact.NomUtilisateurFavoris);
            return View(detailsContact);
        }

        // GET: DetailsContacts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detailsContact = await _context.DetailsContacts.FindAsync(id);
            if (detailsContact == null)
            {
                return NotFound();
            }
            ViewData["NomUtilisateurCreateur"] = new SelectList(_context.Utilisateurs, "NomUtilisateur", "NomUtilisateur", detailsContact.NomUtilisateurCreateur);
            ViewData["NomUtilisateurFavoris"] = new SelectList(_context.Utilisateurs, "NomUtilisateur", "NomUtilisateur", detailsContact.NomUtilisateurFavoris);
            return View(detailsContact);
        }

        // POST: DetailsContacts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NomUtilisateurCreateur,NomUtilisateurFavoris,DateAjout")] DetailsContact detailsContact)
        {
            if (id != detailsContact.NomUtilisateurCreateur)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detailsContact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetailsContactExists(detailsContact.NomUtilisateurCreateur))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["NomUtilisateurCreateur"] = new SelectList(_context.Utilisateurs, "NomUtilisateur", "NomUtilisateur", detailsContact.NomUtilisateurCreateur);
            ViewData["NomUtilisateurFavoris"] = new SelectList(_context.Utilisateurs, "NomUtilisateur", "NomUtilisateur", detailsContact.NomUtilisateurFavoris);
            return View(detailsContact);
        }

        // GET: DetailsContacts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detailsContact = await _context.DetailsContacts
                .Include(d => d.NomUtilisateurCreateurNavigation)
                .Include(d => d.NomUtilisateurFavorisNavigation)
                .FirstOrDefaultAsync(m => m.NomUtilisateurCreateur == id);
            if (detailsContact == null)
            {
                return NotFound();
            }

            return View(detailsContact);
        }

        // POST: DetailsContacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var detailsContact = await _context.DetailsContacts.FindAsync(id);
            _context.DetailsContacts.Remove(detailsContact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetailsContactExists(string id)
        {
            return _context.DetailsContacts.Any(e => e.NomUtilisateurCreateur == id);
        }

        // GET: Recherche Contacts -> work in progress 
        public async Task<IActionResult> SearchForContact()
        {
            var animAlerteContext = _context.DetailsContacts.Where(a => a.NomUtilisateurCreateur == UtilisateursController.usersession).Include(d => d.NomUtilisateurCreateurNavigation).Include(d => d.NomUtilisateurFavorisNavigation);
            return View(await animAlerteContext.ToListAsync());

        }
       
    }
}


//44-47  //var utilisateur = _context.Utilisateurs.SingleOrDefault(a => a.NomUtilisateur == UtilisateursController.usersession && a.UtilisateurActive == 1);
//var animAlerteContext = _context.Utilisateurs.Where(a => a.NomUtilisateur == UtilisateursController.usersession);
//return View(await animAlerteContext.ToListAsync());