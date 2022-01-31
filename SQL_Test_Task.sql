CREATE PROCEDURE testProcedure
(
	@x INT = 1,
	@y INT = 1,
	@z INT = 1000
)
AS BEGIN

DECLARE @BestSellers TABLE
(
	id INT,
	SumByProduct INT,
	TotalPrice MONEY
)

INSERT INTO @BestSellers
	SELECT SubQuery.ProductId, SubQuery.SumByProduct, SubQuery.TotalPrice FROM (
		SELECT pr.ProductId, SUM(o.ProductCount) AS SumByProduct, SUM(o.ProductCount * pr.Price) AS TotalPrice
		FROM OrderItem o JOIN Price pr ON o.PriceId = pr.Id 
		GROUP BY pr.ProductId ) AS SubQuery
	WHERE SubQuery.SumByProduct = 
		(SELECT MAX(SumByProduct) FROM (SELECT pr.ProductId, SUM(o.ProductCount) AS SumByProduct, SUM(o.ProductCount * pr.Price) AS TotalPrice
		FROM OrderItem o JOIN Price pr ON o.PriceId = pr.Id 
		GROUP BY pr.ProductId ) AS SubQuery2)

DECLARE @BestSeller INT
SET @BestSeller = (SELECT id FROM @BestSellers WHERE TotalPrice = 
						(SELECT MAX(TotalPrice) FROM @BestSellers))

SELECT r.RegionName, pro.ProductName, SUM(o.ProductCount) AS ProductSold, COUNT(DISTINCT o.PersonId) AS UniquePerson,
	SUM(o.ProductCount * pr.Price) AS TotalSumByRegion
FROM Region r
	JOIN Person p ON r.Id = p.RegionId
	JOIN OrderItem o ON p.Id = o.PersonId
	JOIN Price pr ON o.PriceId = pr.Id
	JOIN Product pro ON pr.ProductId = pro.Id
WHERE pro.Id = @BestSeller
GROUP BY r.RegionName, pro.ProductName
HAVING SUM(o.ProductCount) >= @x AND
	COUNT(DISTINCT o.PersonId) >= @y AND
	SUM(o.ProductCount * pr.Price) >= @z
ORDER BY r.RegionName

END