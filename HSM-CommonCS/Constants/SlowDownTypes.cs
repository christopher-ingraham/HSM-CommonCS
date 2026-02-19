namespace HSM_CommonCS.Constants;

public enum SlowDownTypes
{
    SLOW_DOWN_TYPE_FULL = 1,              // Full-Speed Slow Down message
    SLOW_DOWN_TYPE_LOW = 2,                             // Low-Speed Slow Down message
    SLOW_DOWN_TYPE_BODY = SLOW_DOWN_TYPE_FULL,
    SLOW_DOWN_TYPE_HEAD = SLOW_DOWN_TYPE_LOW,
    SLOW_DOWN_TYPE_THREADING = 3,
    SLOW_DOWN_TYPE_FLATNESS = 4,
    SLOW_DOWN_TYPE_TAI = 5

}
