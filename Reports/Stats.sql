WITH prs AS (
	SELECT 
		*,
		(julianday(ifnull(datetime(ClosedOn), datetime())) - julianday(CreatedOn)) daysOpen
	FROM 
		[DataSource]
)
SELECT 'Total PRs' Stat, CAST(count(Id) as TEXT) Value FROM prs
UNION ALL
SELECT 'First PR Date', min(date(CreatedOn)) FROM prs
UNION ALL
SELECT 'Last PR Date', max(date(CreatedOn)) FROM prs
UNION ALL
SELECT 'Total Days', round(max(julianDay(CreatedOn)) - min(julianDay(CreatedOn))) FROM prs
UNION ALL
SELECT 'PRs per day', printf('%.1f', count(Id) / round(max(julianDay(CreatedOn)) - min(julianDay(CreatedOn)))) FROM prs
UNION ALL
SELECT 'Average PR Life', printf('%.1f days', avg(daysOpen)) FROM prs 
UNION ALL
SELECT * FROM (SELECT 'Longest Lived PR', printf('%.1f days - %s by %s', daysOpen, Title, CreatedBy) FROM prs ORDER BY daysOpen DESC LIMIT 1) 
UNION ALL
SELECT * FROM (SELECT 'Shortest Lived PR', printf('%.1f minutes - %s by %s', daysOpen * 24 * 60, Title, CreatedBy) FROM prs ORDER BY daysOpen LIMIT 1)
UNION ALL
SELECT * FROM (SELECT 'Shortest PR Completer', printf('%.1f days on average - %s', avg(daysOpen), CreatedBy) from prs GROUP BY CreatedBy ORDER BY avg(daysOpen) LIMIT 1)
UNION ALL
SELECT * FROM (SELECT 'Longest PR Completer', printf('%.1f days on average - %s', avg(daysOpen), CreatedBy) from prs GROUP BY CreatedBy ORDER BY avg(daysOpen) DESC LIMIT 1)
UNION ALL
SELECT * FROM (SELECT 'Top Reviewer', printf('%s voted on %d PRs', Name, count(Id)) FROM PullRequestReviewer WHERE Vote <> 3 GROUP BY Name ORDER BY count(Id) DESC LIMIT 1)