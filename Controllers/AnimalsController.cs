﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnimAlerte.Models;
using Microsoft.AspNetCore.Hosting;
using AnimAlerte.ViewModels;
using System.IO;
using Microsoft.Extensions.Localization;

namespace AnimAlerte.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly IStringLocalizer<AnimalsController> _stringLocalizer;
        private readonly AnimAlerteContext _context;
        private readonly IWebHostEnvironment hosting;


        public AnimalsController(AnimAlerteContext context, IWebHostEnvironment hosting, IStringLocalizer<AnimalsController> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            _context = context;
            this.hosting = hosting;
        }

        // L'affiche initial des animaux pour utilisateur connecté
        public IActionResult Index()
        {
            var animaux = _context.GetAnimalsForUser(UtilisateursController.usersession)
                .OrderByDescending(a => a.DateInscription).ToList();
            ViewBag.images = _context.Images.ToList();
            return View(animaux);
        }

        // Récuperé des animaux par proprietaire
        public IActionResult MesAnimaux()
        {
            var animaux = _context.GetAnimalsForUser(UtilisateursController.usersession)
             .OrderByDescending(a => a.DateInscription).ToList();
            ViewBag.images = _context.Images.ToList();
            return View(animaux);

        }

        // Affiché de l'info d'un animal selectionné
        // GET: Animals2/Details/5
        public async Task<IActionResult> Details(int? idAnimal)
        {
            if (idAnimal == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.ProprietaireNavigation)
               .FirstOrDefaultAsync(m => m.IdAnimal == idAnimal);
            var image = await _context.Images
                .Include(a => a.IdAnimalNavigation)
                .FirstOrDefaultAsync(m => m.IdAnimal == idAnimal);

            var model = new AnimalModifViewModel()
            {
                IdAnimal = animal.IdAnimal,
                NomAnimal = animal.NomAnimal,
                DescriptionAnimal = animal.DescriptionAnimal,
                DateInscription = animal.DateInscription,
                AnimalActif = 1,
                Espece = animal.Espece,
                Proprietaire = animal.Proprietaire,
                PhotoPath = image.PathImage
            };

            if (animal == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // Crée un profil d'un animal
        // GET: Animals/Create
        public IActionResult AjoutAnimal(int idAnimal)
        {

            ViewBag.us = UtilisateursController.usersession;
            var model = new AnimalViewModel
            {
                IdAnimal = idAnimal
            };
            return View(model);
        }

        // POST: Animals/Create
        [HttpPost]
        public async Task<IActionResult> AjoutAnimal(AnimalViewModel model)
        {

            /*  Si une exception dérivée de DbUpdateException est interceptée pendant l'enregistrement des modifications, 
                  un message d'erreur générique s'affiche.*/
            try
            {
                string proprietaire = UtilisateursController.usersession;
                string fileName = ImageUpload(model);
                Animal animal = new();                // Animal animal = new Animal()
                Image image = new();                  // Image image = new Image();

                if (ModelState.IsValid)
                {
                    animal.NomAnimal = model.NomAnimal;
                    animal.DescriptionAnimal = model.DescriptionAnimal;
                    animal.DateInscription = DateTime.Today;
                    animal.AnimalActif = 1;
                    animal.Espece = model.Espece;
                    animal.Proprietaire = proprietaire;

                    _context.Animals.Add(animal);
                    await _context.SaveChangesAsync();

                    image.TitreImage = model.NomAnimal;
                    if (fileName != null)
                    {
                        image.PathImage = fileName;
                    }

                    image.IdAnimal = animal.IdAnimal;
                    _context.Images.Add(image);
                    await _context.SaveChangesAsync();

                    TempData["AlertMessageAnimal"] = _stringLocalizer["Your animal is added successfully !"].Value;

                    return RedirectToAction(nameof(Index), new { msg = ViewBag.Message });
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Impossible d'enregistrer les modifications. " +
                    "Réessayez, et si le problème persiste, " +
                    "consultez votre administrateur système.");
            }

            return View(model);
        }

        // Modifié un animal selectionné
        // GET: Animals/ModifierAnimal
        public async Task<IActionResult> ModifierAnimal(int? idAnimal)
        {
            if (idAnimal == null)
            {
                return NotFound();
            }

            var animalToUpdate = await _context.Animals.FindAsync(idAnimal);
            var imageToUpdate = await _context.Images.FindAsync(idAnimal);
            var model = new AnimalModifViewModel()
            {
                IdAnimal = animalToUpdate.IdAnimal,
                NomAnimal = animalToUpdate.NomAnimal,
                DescriptionAnimal = animalToUpdate.DescriptionAnimal,
                DateInscription = animalToUpdate.DateInscription,
                AnimalActif = 1,
                Espece = animalToUpdate.Espece,
                Proprietaire = animalToUpdate.Proprietaire,
                PhotoPath = imageToUpdate.PathImage
            };

            if (animalToUpdate == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: Animals/ModifierAnimal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifierAnimal(AnimalModifViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Animal animalToUpdate = await _context.Animals.SingleOrDefaultAsync(ani => ani.IdAnimal == model.IdAnimal);
                    animalToUpdate.NomAnimal = model.NomAnimal;
                    animalToUpdate.NomAnimal = model.NomAnimal;
                    animalToUpdate.DescriptionAnimal = model.DescriptionAnimal;
                    animalToUpdate.DateInscription = model.DateInscription;
                    animalToUpdate.AnimalActif = 1;
                    animalToUpdate.Espece = model.Espece;
                    animalToUpdate.Proprietaire = model.Proprietaire;

                    Image imageToUpdate = await _context.Images.SingleOrDefaultAsync(i => i.IdAnimal == model.IdAnimal);

                    if (model.Photo != null)
                    {
                        if (model.PhotoPath != null)
                        {
                            string filePath = Path.Combine(hosting.WebRootPath, "uploads", model.PhotoPath);
                            System.IO.File.Delete(filePath);
                        }
                                           
                        imageToUpdate.PathImage = ImageUpload(model);
                        
                    }

                    _context.Update(animalToUpdate);
                    await _context.SaveChangesAsync();
                    _context.Update(imageToUpdate);
                    await _context.SaveChangesAsync();
                    TempData["AlertMessageAnimal"] = _stringLocalizer["Your animal is modified successfully !"].Value;
                    return RedirectToAction(nameof(MesAnimaux));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Impossible d'enregistrer les modifications. " +
                        "Réessayez, et si le problème persiste, " +
                        "consultez votre administrateur système.");
                }

            }
            return View(model);
        }

        // L'utilisateur désactive son annonce selectionné (Soft delete)
        // GET: Animals/Delete/5
        public async Task<IActionResult> Delete(int? idAnimal, bool? saveChangesError = false)
        {
            if (idAnimal == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals.FindAsync(idAnimal);
            var image = await _context.Images.FindAsync(idAnimal);
            var model = new AnimalModifViewModel()
            {
                IdAnimal = animal.IdAnimal,
                NomAnimal = animal.NomAnimal,
                DescriptionAnimal = animal.DescriptionAnimal,
                DateInscription = animal.DateInscription,
                AnimalActif = 1,
                Espece = animal.Espece,
                Proprietaire = animal.Proprietaire,
                PhotoPath = image.PathImage
            };

            if (model == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Echec de la suppression. Réessayez, et si le problème persiste " +
                    "consultez votre administrateur système.";
            }
            return View(animal);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int idAnimal)
        {
            try
            {
                var animal = await _context.Animals.FindAsync(idAnimal);
                animal.AnimalActif = 0;

                var animalModif = _context.Animals.Attach(animal);
                animalModif.State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["AlertMessageAnimal"] = _stringLocalizer["Your animal is deleted successfully !"].Value;
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { idAnimal = idAnimal, saveChangesError = true });
            }
        }

        //Méthode pour télécharger une image d'un animal
        private string ImageUpload(AnimalViewModel model)
        {
            string nomFichier = null;

            if (model.Photo != null)
            {
                string extFile = Path.GetExtension(model.Photo.FileName);
                string uploadsFolder = Path.Combine(hosting.WebRootPath, "uploads");
                nomFichier = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, nomFichier);
                model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            return nomFichier;
        }

    }
}
