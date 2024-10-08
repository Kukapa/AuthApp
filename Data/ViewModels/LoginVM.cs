﻿using System.ComponentModel.DataAnnotations;

namespace AuthApp.Data.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email Required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


    }
}
