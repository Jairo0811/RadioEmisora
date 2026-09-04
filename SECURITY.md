# Security policy

## Supported version

RadioEmisora RD is a completed portfolio project in maintenance mode.

| Version | Supported |
|---|---|
| 3.1.x | ✅ Security and critical stability fixes |
| 3.0.x | ❌ |
| 2.x and earlier | ❌ |

The active codebase is the WPF/.NET implementation under `RadioEmisoraRD/`. Historical Windows Forms versions are preserved only through Git history.

## Reporting a vulnerability

Please do not publish exploit details in a public issue before a fix is available.

Use GitHub's private vulnerability reporting feature when available for this repository. If private reporting is unavailable, open a public issue containing only a high-level description and request a private contact channel before sharing reproduction details.

A useful report should include:

- affected version;
- affected component;
- impact;
- minimal reproduction conditions;
- whether exploitation requires a malicious catalog, stream endpoint, local file, or network position;
- proposed mitigation, if known.

## Security boundaries

RadioEmisora RD:

- does not implement user accounts or authentication;
- does not store passwords, payment information, or cloud credentials;
- stores preferences, favorites, history, catalog data, and logs locally;
- connects to GitHub-hosted catalog data and third-party streaming endpoints over the network;
- does not proxy or host station audio.

Security reports concerning third-party streaming infrastructure should normally be reported to the corresponding provider rather than this project, unless the issue is caused by how RadioEmisora RD processes that external input.

## Maintenance policy

New feature development is frozen. Fixes may still be accepted when they address:

- a reproducible security vulnerability;
- data loss or corruption;
- application crashes in supported Windows versions;
- regressions in playback or catalog validation;
- broken release/CI infrastructure;
- removal or correction of third-party content when appropriate.
