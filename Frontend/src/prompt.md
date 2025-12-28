You are an expert developer. I will provide you with the output of `git diff --cached`.
Your task is to generate a single-line commit message based on these changes.

Constraints:

1. Format: <type>(frontend): <description>
2. The branch name is 'copilot/refactor-marketing-landing-page', so the 'type' should primarily be 'refactor', unless the diff clearly shows a 'feat' or 'fix'.
3. The scope must always be 'frontend'.
4. The description must be in English, present tense, and should not end with a period.
5. Output ONLY the commit message string, nothing else.

Context: These changes are part of a refactoring process for the marketing landing page.

Diff to analyze:
[INSERT GIT DIFF HERE]
