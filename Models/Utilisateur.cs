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
        [Display(Name="Nom d'utilisateur")]
        public string NomUtilisateur { get; set; }
        public string Nom { get; set; }
        [Display(Name = "Prénon")]
        public string Prenom { get; set; }
        public string Courriel { get; set; }
        [Display(Name = "Mot de passe")]
        public string MotDePasse { get; set; }
        [Display(Name = "Téléphone")]
        public string NumTel { get; set; }
        public byte? UtilisateurActive { get; set; }
        public byte? IsAdmin { get; set; }
        public string NomAdminDesactivateur { get; set; }

        public virtual Administrateur NomAdminDesactivateurNavigation { get; set; }
        public virtual Administrateur Administrateur { get; set; }
        public virtual ICollection<Animal> Animals { get; set; }
        public virtual ICollection<Annonce> Annonces { get; set; }
        public virtual ICollection<DetailsContact> DetailsContactNomUtilisateurCreateurNavigations { get; set; }
        public virtual ICollection<DetailsContact> DetailsContactNomUtilisateurFavorisNavigations { get; set; }
    }
}
