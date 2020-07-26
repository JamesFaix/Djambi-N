# Contributing

## Help wanted

If you'd like to help me build this game, I would greatly appreciate it. Any skill sets or level of experience are welcome. Even if you think you don't have enough experience, there is probably something you can help with.

Here are a few roles where I think the most help is needed:

- Front-end development (TypeScript, React, Redux, CSS)
- Graphic design (Sprites, color palettes, CSS)
- Back-end development (F#, SQL, ASP.NET Core, EntityFramework Core)
- Ops (Docker, AWS, automation)
- Testing (Play the game to find bugs and frustrating parts of gameplay)

## Issues

Issues marked with the label `up-for-grabs` are smaller problems that might be good for a new contributor to tackle. If you are new and something else looks interesting, that's fine. Try it out!

I try to keep GitHub labels, issues, and projects pretty well organized. Reviewing these should give you an idea of where the project is at and what needs to be done.

## Branching strategy

The branching strategy is [GitFlow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow).

- `develop` is the base branch for all feature branches. Pull requests targeting `develop` or merges into `develop` will trigger quality check actions.
- `production` is the GitFlow `master` branch. Soon it will automatically deploy to the production environment after merges.
- `release/{date-created}` branches are branched off of `develop` periodically and merged to `production` when ready.
- `feature/{description and/or issue number}` branches are used for specific issues and merged into `develop`.

## See also

More information is available on the project [wiki](https://github.com/JamesFaix/Apex/wiki).
