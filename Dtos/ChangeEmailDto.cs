namespace UserService.Dtos
{
    public class ChangeEmailDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewEmail { get; set; } = string.Empty;
        public string ConfirmNewEmail { get; set; } = string.Empty;
    }
}