UPDATE Triplogs
SET DaysOut = DATEDIFF(DAY, StartDate, EndDate) + 1
WHERE StartDate IS NOT NULL AND EndDate IS NOT NULL;