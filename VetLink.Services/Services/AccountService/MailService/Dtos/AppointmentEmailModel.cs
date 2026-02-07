namespace VetLink.Services.Services.AccountService.MailService.Dtos
{
    public class AppointmentEmailModel : EmailModel
	{
		public string AppointmentId { get; set; }
		public DateTime AppointmentDate { get; set; }
		public string PetName { get; set; }
		public string VeterinarianName { get; set; }
	}

}
