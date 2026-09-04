# Project status — RadioEmisora RD

**Status:** ✅ Completed / Portfolio-ready  
**Current stable line:** 3.1.x  
**Development mode:** Maintenance only  
**Primary platform:** Windows 10/11 x64  
**Architecture:** WPF + .NET 10 + MVVM

## Decision

RadioEmisora RD is considered functionally complete for its intended scope: a modern reconstruction of the 2018 ITLA final project focused on desktop radio streaming and user experience.

Version 3.1 established the final portfolio baseline with resilient playback, explicit player states, local persistence, remote catalog updates, automated tests, CI, reproducible Windows packages, release artifacts, and portfolio media generated from the real application.

The project will not be expanded into a general-purpose consumer radio platform or SaaS inside this repository. Doing so would change the product domain, architecture, distribution model, and third-party licensing requirements enough to justify a separate product and repository.

## What “complete” means

The project is complete when all of the following are true:

- the application builds in Release mode without warnings;
- automated tests pass;
- portable and self-contained Windows packages can be generated;
- catalog or individual stream failures do not make the entire application unusable;
- local settings, favorites, history, and catalog fallback survive normal failures;
- the README contains current screenshots, installation steps, architecture, troubleshooting, and academic context;
- third-party content is clearly distinguished from the project's own source code;
- security and maintenance expectations are documented.

## Accepted future changes

Maintenance changes remain appropriate for:

- security vulnerabilities;
- data loss or corruption;
- crashes and serious regressions;
- broken streaming compatibility caused by platform changes;
- catalog endpoint maintenance;
- release and CI failures;
- accessibility corrections;
- documentation corrections;
- legitimate third-party removal or attribution requests.

## Changes intentionally out of scope

The following should not be added merely to keep development active:

- user accounts;
- subscriptions or payments;
- advertising systems;
- social networking;
- cloud synchronization;
- SQL databases or a backend without a concrete requirement;
- podcast hosting;
- audio rebroadcast infrastructure;
- multi-tenant station administration;
- large-scale analytics;
- mobile applications inside this WPF repository.

If a future commercial radio platform is created, it should be treated as a separate product that may reuse general engineering lessons from RadioEmisora RD without pretending to be another incremental version of this academic project.

## Portfolio classification

RadioEmisora RD should be presented as a **completed, production-minded portfolio project** demonstrating:

- modernization of legacy software;
- desktop UI engineering with WPF/XAML;
- MVVM separation;
- asynchronous network and media workflows;
- fault-tolerant local persistence;
- testable service design;
- CI/CD and reproducible release packaging;
- maintenance of an external data catalog;
- preservation and modernization of an academic project's original intent.
