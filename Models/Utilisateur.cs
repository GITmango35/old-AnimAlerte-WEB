﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace AnimAlerte.Models
{
    public partial class Utilisateur
    {
        public Utilisateur()
        {
            Animals = new HashSet<Animal>();
            Annonces = new HashSet<Annonce>();
            DetailsContactNomUtilisateurCreateurNavigations = new HashSet<DetailsContact>();
            DetailsContactNomUtilisateurFavorisNavigations = new HashSet<DetailsContact>();
        }

        [StringLength(50)]
        [Display(Name="Nom d'utilisateur")]
        [Required(ErrorMessage = "Entrez un nom d'utilisateur / Enter a username")]
        public string NomUtilisateur { get; set; }
        [StringLength(25)]
        
        [Required(ErrorMessage = "Entrez votre nom / Enter a name")]
        public string Nom { get; set; }
        [StringLength(25)]
        [Required(ErrorMessage = "Entrez votre prénom / Enter a first name")]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }
        [StringLength(25)]
        [Required(ErrorMessage = "Entrez un courriel valide / Enter a valid email")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Courriel non valide / Invalid email")]
        public string Courriel { get; set; }
        [StringLength(25)]
        [Display(Name = "Mot de passe")]
        [Required(ErrorMessage = "Entrez un mot de passe / Enter a password")]
        public string MotDePasse { get; set; }
        [StringLength(10)]
        [Display(Name = "Téléphone")]
        [Required(ErrorMessage = "Entrez votre téléphone / Enter a phone number")]
        [RegularExpression(@"[1-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]$", ErrorMessage = "Format de téléphone non valide / Invalid telephone format")]
        public string NumTel { get; set; }
        [Range(0,1)]
        public byte? UtilisateurActive { get; set; }
        [Range(0, 1)]
        public byte? IsAdmin { get; set; }
        [StringLength(50)]
        public string NomAdminDesactivateur { get; set; }

        public virtual Administrateur NomAdminDesactivateurNavigation { get; set; }
        public virtual Administrateur Administrateur { get; set; }
        public virtual ICollection<Animal> Animals { get; set; }
        public virtual ICollection<Annonce> Annonces { get; set; }
        public virtual ICollection<DetailsContact> DetailsContactNomUtilisateurCreateurNavigations { get; set; }
        public virtual ICollection<DetailsContact> DetailsContactNomUtilisateurFavorisNavigations { get; set; }
    }
}
