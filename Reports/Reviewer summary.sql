SELECT 
	*,
	printf ('%.2f%%', ((1 - (rw.[No Vote] / (rw.[Total PRs] * 1.0))) * 100)) [Vote Rate]
FROM (
	SELECT 
		prr.Name,
		COUNT(DISTINCT prr.PullRequestId) [Total PRs],
		SUM((CASE prr.Vote WHEN 1 THEN 1 ELSE 0 END)) Approved,
		SUM((CASE prr.Vote WHEN 2 THEN 1 ELSE 0 END)) Rejected,
		SUM((CASE prr.Vote WHEN 3 THEN 1 ELSE 0 END)) [No Vote]
	FROM 
		[DataSource] prs
		JOIN PullRequestReviewer prr ON prs.Id = prr.PullRequestId
	GROUP BY 
		prr.Name) rw
ORDER BY
	rw.[Total PRs] DESC