namespace Domain.Models
{
    public class LogAuthenticationState
    {
        public string MtgArenaId { get; private set; } = "";
        public string UserName { get; private set; } = "";

        public void UpdateAuthentication(string mtgArenaId, string playerName)
        {
            MtgArenaId = mtgArenaId;
            UserName = playerName;
        }

        public void Reset()
        {
            MtgArenaId = "";
            UserName = "";
        }
    }
}
