SELECT 
	pr.CreatedBy as User, 
	COUNT(pr.Id) AS [Number of PRs] 
FROM 
	[DataSource] pr 
GROUP BY 
	pr.CreatedBy 
ORDER BY 
	COUNT(pr.Id) DESC