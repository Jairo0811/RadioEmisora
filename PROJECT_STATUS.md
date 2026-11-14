# Project status — RadioEmisora RD

**Status:** ✅ Completed / Portfolio-ready / Maintenance only  
**Stable line:** 3.1.x  
**Final release:** 3.1.1 — Final Portfolio Release  
**Functional scope:** Frozen  
**Recommended platform:** Windows 11 x64 (Windows 10 1809+ remains technically compatible; Windows 10 reached end of general Microsoft support on 14 October 2025)  
**Architecture:** WPF + .NET 10 + MVVM

> Independent academic project originally developed as an ITLA final project. It is not an official application of the Instituto Tecnológico de Las Américas (ITLA).

## Decision

RadioEmisora RD is considered functionally complete for its intended scope: a modern reconstruction of the 2018 ITLA final project focused on desktop radio streaming and user experience.

Version 3.1 established the final portfolio baseline with resilient playback, explicit player states, local persistence, remote catalog updates, automated tests, CI, reproducible Windows packages, release artifacts, and portfolio media generated from the real application.

Version 3.1.1 is the final maintenance release. It adds the tested Windows installer, hardening of untrusted network and file input, NuGet Audit, least-privilege CI/CD, accessibility and UI polish, and documentation synchronized with the code. After 3.1.1 the feature set is frozen.

The project will not be expanded into a general-purpose consumer radio platform or SaaS inside this repository. Doing so would change the product domain, architecture, distribution model, and third-party licensing requirements enough to justify a separate product and repository.

## What “complete” means

The project is complete when all of the following are true:

- the application builds in Release mode without warnings;
- automated tests pass;
- portable, self-contained, and installer packages can be generated and the installer passes its install/upgrade/uninstall smoke test;
- the remote catalog and stream URLs cannot direct the application to local or private network destinations;
- catalog or individual stream failures do not make the entire application unusable;
- local settings, favorites, history, and catalog fallback survive normal failures;
- the README contains current screenshots, installation steps, architecture, troubleshooting, and academic context;
- third-party content is clearly distinguished from the project's own source code;
- security and maintenance expectations are documented.

## Accepted future changes

From 3.1.1 onward, only these changes are accepted (as 3.1.x patch releases):

- security vulnerabilities;
- crashes and serious regressions;
- data loss or corruption;
- stream and catalog maintenance (dead or moved endpoints);
- compatibility with supported Windows and .NET versions;
- release and CI failures;
- accessibility corrections;
- documentation corrections;
- legitimate third-party removal or attribution requests.

## External items

- A trusted Authenticode code-signing certificate is not part of the repository. Releases are built unsigned unless the `WINDOWS_SIGNING_CERT_BASE64` and `WINDOWS_SIGNING_CERT_PASSWORD` secrets are configured; unsigned packages may trigger SmartScreen warnings.
- Stream availability depends on each broadcaster or provider. At the 3.1.1 release, 11 of the 12 catalog streams answered with audio; Radio Popular 950 AM's provider endpoint (surfernetwork) returned `503 Service Unavailable` and no verifiable official alternative was found, so the entry is kept unchanged and the application shows its normal "unavailable / retry" state for it.

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
- CI/CD, least-privilege release automation, and a tested Windows installer;
- pragmatic security hardening of untrusted input;
- maintenance of an external data catalog;
- preservation and modernization of an academic project's original intent.
