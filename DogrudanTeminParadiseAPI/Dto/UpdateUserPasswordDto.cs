﻿namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateUserPasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
