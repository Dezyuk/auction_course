using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Services
{
    public interface IAuctionService
    {
        List<AuctionResponse> GetPastAuctions();
        List<AuctionResponse> GetActiveAuctions();
        List<AuctionResponse> GetFutureAuctions();
        Guid CreateAuction(AuctionCreateRequest request);
        AuctionResponse GetAuctionById(Guid id);
        void UpdateAuction(Guid id, AuctionUpdateRequest request);
        void DeleteAuction(Guid id);
    }
}
