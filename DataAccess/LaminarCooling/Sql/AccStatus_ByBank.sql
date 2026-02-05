SELECT 
AREA_ID				as AreaId,
CENTER_ID			as CenterId,
ZONE_NO				as ZoneNo,
ZONE_ID				as ZoneId,
BANK_SEQ			as BankSeq,
BANK_OUT_OF_ORDER	as BankOutOfOrder,
DEVICE_OUT_OF_ORDER	as DeviceOutOfOrder,
RTDB_ACC_STATUS_NO	as RtdbAccStatusNo
FROM RTDB_ACC_STATUS
WHERE BANK_NO    = :bankNo
  AND BANK_POS   = :bankPos
  AND DEVICE_TYPE = :deviceType
