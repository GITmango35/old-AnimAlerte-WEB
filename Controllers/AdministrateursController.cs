﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AnimAlerte.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace AnimAlerte.Controllers
{
    public class AdministrateursController : Controller
    {
        private readonly AnimAlerteContext _context;
        private readonly IHtmlLocalizer<AdministrateursController> _localizer;
        private readonly IStringLocalizer<AdministrateursController> _stringLocalizer;

        public AdministrateursController(AnimAlerteContext context, IHtmlLocalizer<AdministrateursController> localizer, IStringLocalizer<AdministrateursController> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            _context = context;
            _localizer = localizer;

        }

        // GET: Administrateurs
        public async Task<IActionResult> Index()
        {
            var animAlerteContext = _context.Administrateurs.Include(a => a.NomAdminNavigation);
            return View(await animAlerteContext.ToListAsync());
        }

        // GET: Administrateurs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrateur = await _context.Administrateurs
                .Include(a => a.NomAdminNavigation)
                .FirstOrDefaultAsync(m => m.NomAdmin == id);
            if (administrateur == null)
            {
                return NotFound();
            }

            return View(administrateur);
        }
        // GET: Utilisateurs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Utilisateurs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("NomUtilisateur,Nom,Prenom,Courriel,MotDePasse,NumTel,UtilisateurActive,IsAdmin,NomAdminDesactivateur")] Utilisateur utilisateur)
        {
            var u = _context.Utilisateurs.FirstOrDefault(u => u.NomUtilisateur == utilisateur.NomUtilisateur);
            ViewBag.Message = "";
            Utilisateur obj = new Utilisateur();
            try
            {
                if (u == null && ModelState.IsValid)
                {
                    obj.NomUtilisateur = utilisateur.NomUtilisateur;
                    obj.Nom = utilisateur.Nom;
                    obj.Prenom = utilisateur.Prenom;
                    obj.Courriel = utilisateur.Courriel;
                    obj.MotDePasse = utilisateur.MotDePasse;
                    obj.NumTel = utilisateur.NumTel;
                    obj.UtilisateurActive = 1;
                    obj.IsAdmin = 1;
                    obj.NomAdminDesactivateur = null;
                    _context.Utilisateurs.Add(obj);
                    _context.SaveChanges();
                    Administrateur admin = new Administrateur();
                    admin.NomAdmin = utilisateur.NomUtilisateur;
                    admin.DateCreation = DateTime.Today;
                    _context.Administrateurs.Add(admin);
                    _context.SaveChanges();
                    TempData["AlertAddAdmin"] = _stringLocalizer["The addmin is added successfully !"].Value;
                    var message = _localizer["Registered"];
                    ViewBag.Message = message;
                    return RedirectToAction("AllAnnoncesAdmin", "Annonces", new { msg = ViewBag.Message });
                }
                else
                {
                    var message = _localizer["AlreadyRegistered"];
                    ViewBag.Message = message;
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Administrateurs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrateur = await _context.Administrateurs.FindAsync(id);
            if (administrateur == null)
            {
                return NotFound();
            }
            ViewData["NomAdmin"] = new SelectList(_context.Utilisateurs, "NomUtilisateur", "NomUtilisateur", administrateur.NomAdmin);
            return View(administrateur);
        }

        // POST: Administrateurs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NomAdmin,DateCreation")] Administrateur administrateur)
        {
            if (id != administrateur.NomAdmin)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(administrateur);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdministrateurExists(administrateur.NomAdmin))
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
            ViewData["NomAdmin"] = new SelectList(_context.Utilisateurs, "NomUtilisateur", "NomUtilisateur", administrateur.NomAdmin);
            return View(administrateur);
        }

        // GET: Administrateurs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrateur = await _context.Administrateurs
                .Include(a => a.NomAdminNavigation)
                .FirstOrDefaultAsync(m => m.NomAdmin == id);
            if (administrateur == null)
            {
                return NotFound();
            }

            return View(administrateur);
        }

        // POST: Administrateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var administrateur = await _context.Administrateurs.FindAsync(id);
            _context.Administrateurs.Remove(administrateur);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdministrateurExists(string id)
        {
            return _context.Administrateurs.Any(e => e.NomAdmin == id);
        }
    }
}
