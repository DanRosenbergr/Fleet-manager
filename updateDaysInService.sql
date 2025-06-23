UPDATE Repairs
SET DaysInService = DATEDIFF(DAY, RepairDateStart, RepairDateEnd) + 1
WHERE RepairDateStart IS NOT NULL AND RepairDateEnd IS NOT NULL;