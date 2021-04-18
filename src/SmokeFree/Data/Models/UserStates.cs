namespace SmokeFree.Data.Models
{
    /// <summary>
    /// User States Througout App
    /// </summary>
    public enum UserStates
    {
        Initial,
        CompletedOnBoarding,
        UserUnderTesting,
        IsTestComplete,
        InCreateChallenge,
        InChallenge,
        Complete
    }
}
