namespace KingGuardians.Cards
{
    public interface ICardProvider
    {
        CardDefinition GetCardAt(int handIndex);
    }
}
