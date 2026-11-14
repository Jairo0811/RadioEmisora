# Security policy

## Supported version

RadioEmisora RD is a completed portfolio project in maintenance mode.

| Version | Supported |
|---|---|
| 3.1.x (latest: 3.1.1) | ✅ Security and critical stability fixes |
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

## Threat model and mitigations

The only untrusted inputs are the remote catalog (`catalog/emisoras.json`, fetched from GitHub), the stream endpoints it lists, and local JSON files. The goal is that a compromised catalog cannot make the application reach internal destinations or process absurd input.

| Risk | Mitigation |
|---|---|
| SSRF to `localhost`, `127.0.0.1`, `::1`, RFC 1918, link-local (`169.254.0.0/16`, `fe80::/10`), CGNAT, ULA, multicast | `NetworkUriPolicy` accepts only public HTTPS destinations; blocked ranges include NAT64/6to4 addresses embedding internal IPv4 |
| Local host names (`.local`, `.internal`, `.lan`, `.home.arpa`, single-label names) | Rejected before any request |
| DNS rebinding | The resolved IP is validated in the socket connect callback, and the connection uses that same IP |
| Redirects to internal or HTTP destinations | Redirects are followed manually (max. 5) and every hop is validated; an `http://` hop is upgraded to `https://` before validation, so no plain-HTTP connection is ever opened |
| Credentials embedded in URLs, overlong URLs | Rejected (`user:pass@`, more than 2048 characters, control characters) |
| Huge or deeply nested JSON | Remote catalog capped at 512 KB even when chunked or compressed; local JSON capped at 1 MB; maximum depth 16 |
| Malicious strings and IDs | Length limits, control characters rejected, strict ID/color patterns |
| Path traversal through logo names | Logos must match `[a-z0-9-].png` and are loaded only from embedded application resources |
| Log flooding or log injection | One line per event, message length limits, size rotation and file retention |
| Vulnerable dependencies | NuGet Audit (direct and transitive); moderate, high and critical advisories fail the build |

Residual risks, accepted for a local desktop client:

- When Windows is configured to use a proxy, the connection is made to that proxy chosen by the user or organization; destinations are still validated by name.
- After validation, WPF `MediaPlayer` opens the validated stream URL with its own resolver. A DNS rebinding in that short window could at most make the media engine issue an audio request; no response is exposed to the catalog author.
- Stream URLs must use HTTPS; all current catalog stations do.

### Decision on HTTP

Three catalog stations (radiojar) answer the HTTPS request with a redirect to `http://nXX.radiojar.com/...` with a short-lived token, although the same URL is also served over HTTPS with a valid certificate. Instead of allowing HTTP or dropping those stations, RadioEmisora RD upgrades `http://` redirect hops to `https://` (as browsers do for mixed audio content). If a destination does not support HTTPS, playback fails safely; the application never downgrades to HTTP.

Security reports concerning third-party streaming infrastructure should normally be reported to the corresponding provider rather than this project, unless the issue is caused by how RadioEmisora RD processes that external input.

## Privacy

RadioEmisora RD does not create accounts, collect telemetry or analytics, store passwords, or send favorites or history anywhere. Its only network requests are the catalog download and the streams the user chooses to play.

## Maintenance policy

New feature development is frozen since 3.1.1. Fixes may still be accepted when they address:

- a reproducible security vulnerability;
- data loss or corruption;
- application crashes in supported Windows versions;
- regressions in playback or catalog validation;
- broken release/CI infrastructure;
- removal or correction of third-party content when appropriate.
