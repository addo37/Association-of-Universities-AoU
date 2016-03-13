using ProtoBuf;

namespace ServerData
{
    public enum PacketType
    {
        Connect,
        Login,
        Contributions,
        Funds,
        Exchanges,
        Scholarships,
        Users,
        Events,
        AcceptScholarship,
        RejectScholarship,
        AcceptExchange,
        RejectExchange,
        AcceptFund,
        RejectFund,
        AcceptContribution,
        RejectContribution,
        Chat,
        AddUser,
        UpdateLists,
        DemoteUser,
        PromoteUser,
        DeleteUser,
        GetLogs,
        AddEvent,
        Token,
        Reset,
        TokenMismatch,
        ResetSuccess,
    }
}
