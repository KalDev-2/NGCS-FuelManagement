﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FCS.ViewModels;
public class ForgotPasswordViewModel
{
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
}
