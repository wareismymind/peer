# Changelog

### [1.7.4](https://www.github.com/wareismymind/peer/compare/v1.7.3...v1.7.4) (2022-01-26)


### Bug Fixes

* handle errors in show/watch ([#147](https://www.github.com/wareismymind/peer/issues/147)) ([8247867](https://www.github.com/wareismymind/peer/commit/824786781edf230f7e83032f59b0fe5aafcd567a))

### [1.7.3](https://www.github.com/wareismymind/peer/compare/v1.7.2...v1.7.3) (2022-01-06)


### Bug Fixes

* when a user is deleted the Author field isn't returned. replacing author with 'octoghost' in those cases ([#143](https://www.github.com/wareismymind/peer/issues/143)) ([eb00ab0](https://www.github.com/wareismymind/peer/commit/eb00ab0e3ea00411e973fc324b1d49a6445facfa))

### [1.7.2](https://www.github.com/wareismymind/peer/compare/v1.7.1...v1.7.2) (2022-01-04)


### Bug Fixes

* during the changes to status fetching the rollup got lost along the way ([#141](https://www.github.com/wareismymind/peer/issues/141)) ([187f3d9](https://www.github.com/wareismymind/peer/commit/187f3d9cf318aa49563370c65c1883cedc165975))

### [1.7.1](https://www.github.com/wareismymind/peer/compare/v1.7.0...v1.7.1) (2021-12-31)


### Bug Fixes

* accept filter contents with embedded colons (like 'title:fix:') ([#139](https://www.github.com/wareismymind/peer/issues/139)) ([fe138dd](https://www.github.com/wareismymind/peer/commit/fe138ddd22f259d4245e792e73dd3008583be47c))

## [1.7.0](https://www.github.com/wareismymind/peer/compare/v1.6.0...v1.7.0) (2021-12-31)


### Features

* add initial filter impl ([#137](https://www.github.com/wareismymind/peer/issues/137)) ([9de2b55](https://www.github.com/wareismymind/peer/commit/9de2b552cde8a412963008577f836c5b6df58ada))
* add sort keys to show help ([#132](https://www.github.com/wareismymind/peer/issues/132)) ([b9caa76](https://www.github.com/wareismymind/peer/commit/b9caa76108cf282d49ae3af7f546acd49fcc1c74))

## [1.6.0](https://www.github.com/wareismymind/peer/compare/v1.5.2...v1.6.0) (2021-12-24)


### Features

* async enumerable and lazy paging ([#128](https://www.github.com/wareismymind/peer/issues/128)) ([44a0620](https://www.github.com/wareismymind/peer/commit/44a062087bf98145fcc6b2221eaa34a89a26c430))

### [1.5.2](https://www.github.com/wareismymind/peer/compare/v1.5.1...v1.5.2) (2021-12-23)


### Bug Fixes

* config would fail because no providers were configured ([#129](https://www.github.com/wareismymind/peer/issues/129)) ([143f16e](https://www.github.com/wareismymind/peer/commit/143f16e9b49b752f425d0d1698ac0bf7fc865919))

### [1.5.1](https://www.github.com/wareismymind/peer/compare/v1.5.0...v1.5.1) (2021-12-19)


### Bug Fixes

* help text was always being run even on successful execution and caused error ([#126](https://www.github.com/wareismymind/peer/issues/126)) ([fdd5ff0](https://www.github.com/wareismymind/peer/commit/fdd5ff04c0ad4af3d06a851cbc5c476dac8e0503))

## [1.5.0](https://www.github.com/wareismymind/peer/compare/v1.4.0...v1.5.0) (2021-12-19)


### Features

* better help ([#124](https://www.github.com/wareismymind/peer/issues/124)) ([721591e](https://www.github.com/wareismymind/peer/commit/721591e7698d92ce6c05e4e622f529efbfe4636d))
* intial shot at the details command ([#119](https://www.github.com/wareismymind/peer/issues/119)) ([28087e8](https://www.github.com/wareismymind/peer/commit/28087e83c2d2f16618d9a1421dc9c4812a151a76))

## [1.4.0](https://www.github.com/wareismymind/peer/compare/v1.3.3...v1.4.0) (2021-10-30)


### Features

* initial implementation of watch on show ([#110](https://www.github.com/wareismymind/peer/issues/110)) ([1421f11](https://www.github.com/wareismymind/peer/commit/1421f11347d42ee32328361fb2876de67a2d7b2b))
* making page size configurable ([#83](https://www.github.com/wareismymind/peer/issues/83)) ([a671f04](https://www.github.com/wareismymind/peer/commit/a671f04e406b037ecda4b730471dd574261c509b))


### Bug Fixes

* factor commands into classes ([#107](https://www.github.com/wareismymind/peer/issues/107)) ([a52038c](https://www.github.com/wareismymind/peer/commit/a52038cb00934465cbf36e73f8861517d7f3ae99))

### [1.3.3](https://www.github.com/wareismymind/peer/compare/v1.3.2...v1.3.3) (2021-10-25)


### Bug Fixes

* remove rids and self contained from project file for now ([12cb8b3](https://www.github.com/wareismymind/peer/commit/12cb8b3c47246eb5af776ebd2ad982a267fc59ca))

### [1.3.2](https://www.github.com/wareismymind/peer/compare/v1.3.1...v1.3.2) (2021-10-25)


### Bug Fixes

* add project to build ([4759aa1](https://www.github.com/wareismymind/peer/commit/4759aa1cd22e56c31f541756d5fde873ad6d0bec))

### [1.3.1](https://www.github.com/wareismymind/peer/compare/v1.3.0...v1.3.1) (2021-10-25)


### Miscellaneous Chores

* release 1.3.1 ([0435151](https://www.github.com/wareismymind/peer/commit/0435151b43bd5c94a10b388f9f0214af5927778b))

## [1.3.0](https://www.github.com/wareismymind/peer/compare/v1.2.0...v1.3.0) (2021-10-25)


### Features

* added sorting to the show view ([#98](https://www.github.com/wareismymind/peer/issues/98)) ([a696c5a](https://www.github.com/wareismymind/peer/commit/a696c5a842584694c76f1fc5946a245314c1b1b4))
* also query review-requested separately so we get all the prs ([#87](https://www.github.com/wareismymind/peer/issues/87)) ([065910b](https://www.github.com/wareismymind/peer/commit/065910b9593f6a3046b842467f8612eab8651bdf))
* update methods to accept/utilize cancellationtoken in network deps ([#86](https://www.github.com/wareismymind/peer/issues/86)) ([8fa0063](https://www.github.com/wareismymind/peer/commit/8fa0063951fa3d3e19c5238fcf533bc76ace9b95))


### Bug Fixes

* Fix path seperator ([#76](https://www.github.com/wareismymind/peer/issues/76)) ([a26e986](https://www.github.com/wareismymind/peer/commit/a26e986fddbb0b26f098ad648c2c7c2f7b8a1e28))
* Made resolved comments a calculated property ([#85](https://www.github.com/wareismymind/peer/issues/85)) ([ba4dcce](https://www.github.com/wareismymind/peer/commit/ba4dcce9419f04c645972953f4bbb14fb4da8fcc))

## [1.2.0](https://www.github.com/wareismymind/peer/compare/v1.1.9...v1.2.0) (2021-10-22)


### Features

* add extension to name in release ([378886e](https://www.github.com/wareismymind/peer/commit/378886e574e34b3a73455d41e45d3e34bbbaf8a3))

### [1.1.9](https://www.github.com/wareismymind/peer/compare/v1.1.8...v1.1.9) (2021-10-21)


### Bug Fixes

* use @ in filename and build filename instead of gci ([a3ce160](https://www.github.com/wareismymind/peer/commit/a3ce160abceab62627b2f12c9de51a0f72a2cb5e))

### [1.1.8](https://www.github.com/wareismymind/peer/compare/v1.1.7...v1.1.8) (2021-10-21)


### Bug Fixes

* ls -> gci ([79d12ae](https://www.github.com/wareismymind/peer/commit/79d12ae1db5582177db58b6de825a9c754e1b137))

### [1.1.7](https://www.github.com/wareismymind/peer/compare/v1.1.6...v1.1.7) (2021-10-21)


### Bug Fixes

* fix assembly name ([402e249](https://www.github.com/wareismymind/peer/commit/402e24993fde518ffd62793a3c58ff7f3f926f0c))

### [1.1.6](https://www.github.com/wareismymind/peer/compare/v1.1.5...v1.1.6) (2021-10-21)


### Bug Fixes

* upload to gh via curl probably ([#72](https://www.github.com/wareismymind/peer/issues/72)) ([4e8989e](https://www.github.com/wareismymind/peer/commit/4e8989e15e07c98c52cfa4d205c02cca7998fd15))

### [1.1.5](https://www.github.com/wareismymind/peer/compare/v1.1.4...v1.1.5) (2021-10-21)


### Bug Fixes

* mac and linux had some troubles with localization support in com… ([#68](https://www.github.com/wareismymind/peer/issues/68)) ([747eab2](https://www.github.com/wareismymind/peer/commit/747eab29c925e670120c65ff48df44632a8832c0))

### [1.1.4](https://www.github.com/wareismymind/peer/compare/v1.1.2...v1.1.4) (2021-10-21)


### Miscellaneous Chores

* release 1.1.3 ([84a01d3](https://www.github.com/wareismymind/peer/commit/84a01d38e9b100ea71a595154e0122a59aaf952d))
* release 1.1.4 ([de30c12](https://www.github.com/wareismymind/peer/commit/de30c12f692eb936901612dc9d190e035cba2b1d))

### [1.1.2](https://www.github.com/wareismymind/peer/compare/v1.1.2...v1.1.2) (2021-10-21)


### Miscellaneous Chores

* release 1.1.2 ([76675f0](https://www.github.com/wareismymind/peer/commit/76675f0a439c8d9a64886c28e3abe5af95206cde))

### [1.1.2](https://www.github.com/wareismymind/peer/compare/v1.1.1...v1.1.2) (2021-10-21)


### Bug Fixes

* only run nuget build/push on release ([#64](https://www.github.com/wareismymind/peer/issues/64)) ([a6e5387](https://www.github.com/wareismymind/peer/commit/a6e538747708720fa59a887df35f9a7fde81a886))

### [1.1.1](https://www.github.com/wareismymind/peer/compare/v1.1.0...v1.1.1) (2021-10-21)


### Bug Fixes

* specify source in build and make nuget publish its own thing outside of matrix builds ([#61](https://www.github.com/wareismymind/peer/issues/61)) ([e0af8b5](https://www.github.com/wareismymind/peer/commit/e0af8b56d21e1b8400f462ac9c42d58349b8d93a))
* yaml whitespace error ([#62](https://www.github.com/wareismymind/peer/issues/62)) ([42581b1](https://www.github.com/wareismymind/peer/commit/42581b1dc86b74a497b6b2653bf9a85ff30577b2))

## [1.1.0](https://www.github.com/wareismymind/peer/compare/v1.0.1...v1.1.0) (2021-10-21)


### Features

* add-basic-config-and-config-failure-logging ([#53](https://www.github.com/wareismymind/peer/issues/53)) ([35f4e4e](https://www.github.com/wareismymind/peer/commit/35f4e4e10851634bc20a84934528ac80140fe716))
* Change 'comments' to the speech balloon emoji ([#57](https://www.github.com/wareismymind/peer/issues/57)) ([2042c0c](https://www.github.com/wareismymind/peer/commit/2042c0ce88ea227cddece656d057cbcae82dc4cf))

### [1.0.1](https://www.github.com/wareismymind/peer/compare/v1.0.0...v1.0.1) (2021-10-19)


### Bug Fixes

* fix workflow file dependency ([#44](https://www.github.com/wareismymind/peer/issues/44)) ([f5ef8ea](https://www.github.com/wareismymind/peer/commit/f5ef8eab56ed8ea3a7cf3b823fadc4753dba8428))

## 1.0.0 (2021-10-19)


### Features

* add build test and publish pipes ([#19](https://www.github.com/wareismymind/peer/issues/19)) ([ac5d95a](https://www.github.com/wareismymind/peer/commit/ac5d95abd1f1cb1c283b33b08412d0686971b6d5))
* add open command and identifiers to pull requests ([#40](https://www.github.com/wareismymind/peer/issues/40)) ([28611fd](https://www.github.com/wareismymind/peer/commit/28611fd189987e945bf3ca1e6c9d41450938ec5f))
* adding contributing.md ([#27](https://www.github.com/wareismymind/peer/issues/27)) ([f6a941f](https://www.github.com/wareismymind/peer/commit/f6a941f368f1c2c505eaebed9cf1bb4d96eb7524))
* Config loading ([#26](https://www.github.com/wareismymind/peer/issues/26)) ([679bc28](https://www.github.com/wareismymind/peer/commit/679bc283b6722dedbd0c6e4120e53cf7c4a49ca7))
* console-writer implementation ([5ee53f5](https://www.github.com/wareismymind/peer/commit/5ee53f5e4f6bb9d406371d15d0c04ab05f4fd7b9))
* Factor out GQL models ([#36](https://www.github.com/wareismymind/peer/issues/36)) ([f850cdb](https://www.github.com/wareismymind/peer/commit/f850cdb54faa2b68ecb9d18a5d99771ca224085b))
* Get username from API if not configured ([#38](https://www.github.com/wareismymind/peer/issues/38)) ([6c62490](https://www.github.com/wareismymind/peer/commit/6c62490bf39e95008aa8d4bfcd8486f2f9d7d016))
* initial console application ([172dc4f](https://www.github.com/wareismymind/peer/commit/172dc4f4070c24a8bcf99c7153bd7f5183d3a32c))
* main show loop ([#35](https://www.github.com/wareismymind/peer/issues/35)) ([b42eb80](https://www.github.com/wareismymind/peer/commit/b42eb80258c1bf2e7f8ab4e15f832b7a02b32879))
* Populate status ([#41](https://www.github.com/wareismymind/peer/issues/41)) ([93df4fc](https://www.github.com/wareismymind/peer/commit/93df4fc369dc64425376f68a4fe303eb2bf224a9))
* PR objects and object structure ([2dadce1](https://www.github.com/wareismymind/peer/commit/2dadce12b32ebd9d16ee61b9363d8286f94eb0ec))
* tests and impl of the compact formatter type. ([aa3ef42](https://www.github.com/wareismymind/peer/commit/aa3ef422bd9e419b24cd03e285552077febbf78a))


### Bug Fixes

* build-fixes ([#20](https://www.github.com/wareismymind/peer/issues/20)) ([4b62311](https://www.github.com/wareismymind/peer/commit/4b62311ae905a17f67e663a8ca4e3579b1ffea2b))
* Fix negation syntax in GH search ([#39](https://www.github.com/wareismymind/peer/issues/39)) ([5e59a56](https://www.github.com/wareismymind/peer/commit/5e59a562c32c394399f82dbd9e5d0d64c49a5199))
* runs on typo ([#21](https://www.github.com/wareismymind/peer/issues/21)) ([14063e8](https://www.github.com/wareismymind/peer/commit/14063e81ae4a90170a48f1cced4fa39e80d16638))
