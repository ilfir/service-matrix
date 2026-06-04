# Scratchpad - Version Controller Fix

## Objective
Fix the hardcoded GitSha in VersionController to show the current commit SHA and remove the /version/sha endpoint.

## Problem
The hardcoded GitSha (`7a4eada0f8a45b44fb4b493bc47c18b1d0dad201`) was from an old commit. GitHub showed a different hash because:
- The `main` branch on GitHub is at commit `97dfd9` (Merge pull request #6)
- The local `feature/improvements-from-plan` branch HEAD is at `5ff316a`
- The hardcoded value was from the "Exception middleware" commit (`7a4eada`)

## Changes Made
1. **API/Controllers/VersionController.cs**: 
    - Removed the `/version/sha` endpoint (only `/version` remains)
    - Changed `ConfigurationService.GitSha` from `const string` to dynamically computed value
    - Added runtime resolution: reads `.git-sha` file if present, falls back to `git rev-parse HEAD` command
2. **API/service-matrix.csproj**: Kept clean (no build-time target needed since runtime resolution works)
3. **.gitignore**: Added `*.git-sha` pattern

## Verification
- Build: succeeded with 22 warnings (pre-existing, unrelated)
- Tests: 117 passed, 0 failed, 0 skipped

## Notes
- The `/version` endpoint now returns the actual current commit SHA
- For CI/CD deployments, you can optionally generate a `.git-sha` file as part of the deploy pipeline