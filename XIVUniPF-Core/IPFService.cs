namespace XIVUniPF_Core
{
    public interface IPFService
    {
        public delegate void PartyFinderUpdateEventHandler(IPFService sender);

        public event PartyFinderUpdateEventHandler? OnPartyFinderUpdate;

        public Task Refresh(Action<float>? progressCallback = null, bool useProxy = true);

        public PartyList GetParties();
    }
}
