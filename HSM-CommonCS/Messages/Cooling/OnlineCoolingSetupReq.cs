using HSM_CommonCS.Constants;

namespace HSM_CommonCS.Messages.Cooling;

public sealed record OnlineCoolingSetupReq : IL2L2Message
{
    public const int MessageId = 2022;
    int IL2L2Message.Id => MessageId;

    public int InPieceNo { get; init; }               // Input Piece No
    public string InPieceId { get; init; }            // Input Piece ID

    public int SetupMode { get; init; }                // Setup Mode (from tables/from model)
    public int SetupCount { get; init; }                 // Setup Counter from Mill

    public SetupCalcType SetupCalcType { get; init; }


    short PreliminaryReq { get; init; }                          // [-] Flag for preliminary or definitive (target vs calculated speed) calculation (0 - 1), not used, to evaluate

    double EntryROTPyroTemp { get; init; }                       // [degC] Temperature at mill exit (ROT entry) => entry ROT pyrometer, calculated by mill setup, not used for setup calculation, used target one, to evaluate 
    double LastFMStandSpeed { get; init; }                          // [m/s]  Speed at finishing mill last stand
    double Acceleration { get; init; }                           // [m/s2] Acceleration of the strip (not used)
}
