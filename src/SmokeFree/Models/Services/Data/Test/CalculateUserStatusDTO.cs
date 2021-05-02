using SmokeFree.Data.Models;

namespace SmokeFree.Models.Services.Data.Test
{
    /// <summary>
    /// TestCalculationService - CalculateUserStatus Response Model
    /// </summary>
    public class CalculateUserStatusDTO
    {
        public CalculateUserStatusDTO(bool success)
        {
            Success = success;
        }

        public CalculateUserStatusDTO(bool success, string message) : this(success)
        {
            Success = success;
        }

        public CalculateUserStatusDTO(bool success, UserSmokeStatuses status) : this(success)
        {
         
            Status = status;
        }



        public bool Success { get; }
        public string Message { get; }
        public string Icon { get; }
        public string Text { get; }
        public UserSmokeStatuses Status { get; }
    }
}
