# YAML → CI/CD Pipeline: Beginner to Intermediate

A complete, self-contained tutorial. Two parts:

1. **Part A — YAML itself** (the language your pipeline files are written in)
2. **Part B — CI/CD pipelines** (using that YAML to build real pipelines)

Work top to bottom. Every concept builds on the previous one.

---

# PART A — YAML Fundamentals

YAML is just a way to write **data**: lists, key-value pairs, and nesting. That's it. It is *not* a programming language — no loops, no logic. A pipeline tool (GitHub Actions, GitLab, Azure DevOps) *reads* your YAML and does what it describes.

**Analogy:** YAML is a recipe card. It lists ingredients and steps in a structured way. The chef (the CI tool) reads the card and cooks. The card itself does nothing.

## 1. The two building blocks

### Key-value pairs (a "mapping")

```yaml
name: My Pipeline
version: 8
enabled: true
```

- `key: value` — note the **space after the colon**. `key:value` is wrong.
- Strings usually don't need quotes: `name: My Pipeline` is fine.

### Lists (a "sequence")

```yaml
fruits:
  - apple
  - banana
  - cherry
```

- Each `-` is one list item.
- The indentation matters (see next section).

## 2. Indentation is everything

YAML uses **spaces** (never tabs) to show nesting — like how a bullet-point outline shows hierarchy.

```yaml
person:
  name: Aziz
  skills:
    - dotnet
    - devops
  address:
    city: Dhaka
    country: Bangladesh
```

Reading this: `person` contains `name`, `skills` (a list), and `address` (which itself contains `city` and `country`).

**Rules:**
- Use **2 spaces** per level (a common convention). Be consistent.
- **Never use tabs.** This is the #1 beginner error. Configure your editor to insert spaces.
- Alignment shows relationship: things at the same indent level are siblings.

## 3. Nesting lists and maps together

This is where real pipeline files live. Combine the two building blocks:

```yaml
steps:
  - name: Checkout
    uses: actions/checkout@v4
  - name: Build
    run: dotnet build
```

Reading this: `steps` is a **list**. Each item is a **map** with keys `name` and one of (`uses` / `run`). The `-` marks where each new item starts; keys under it (aligned) belong to that item.

**A common confusion — spot the difference:**

```yaml
# A list of two maps
steps:
  - name: Build
    run: dotnet build

# One map with two keys (NO dashes)
step:
  name: Build
  run: dotnet build
```

## 4. Data types

```yaml
a_string: hello
a_number: 42
a_float: 3.14
a_boolean: true          # also: false, yes, no, on, off
a_null: null             # also: ~
quoted_string: "true"    # forces it to be text, not boolean
```

**Gotcha:** unquoted `no`, `yes`, `on`, `off` become booleans. If you literally mean the word "no", quote it: `answer: "no"`.

## 5. Multi-line strings

You'll need these for shell scripts inside pipelines.

```yaml
# Literal block "|" — keeps line breaks
script: |
  echo "Line 1"
  echo "Line 2"
  dotnet test

# Folded block ">" — folds newlines into spaces (one long line)
description: >
  This is all
  one single line
  when parsed.
```

Use `|` for multi-command shell scripts (you want each command on its own line).

## 6. Comments, documents, and anchors

```yaml
# This is a comment. Everything after # is ignored.

name: value   # inline comment
```

**Anchors (`&`) and aliases (`*`)** — reuse a block to avoid repetition (DRY):

```yaml
defaults: &default-settings
  timeout: 30
  retries: 3

job_a:
  <<: *default-settings   # merges default-settings in here
  name: Job A

job_b:
  <<: *default-settings
  name: Job B
```

Both jobs inherit `timeout` and `retries`. You'll see this in advanced pipelines.

## 7. YAML syntax checklist (memorize)

- Space after every colon: `key: value`
- 2 spaces per indent level, spaces only, **no tabs**
- `-` for list items
- Same indentation = siblings
- Quote strings that look like booleans/numbers when you mean text
- `#` for comments
- `|` keeps line breaks, `>` folds them

**Tip:** paste your YAML into a validator (search "YAML lint") when learning. Editors with a YAML extension will underline errors live.

---

# PART B — CI/CD Pipelines

Now we use YAML to describe pipelines. I'll use **GitHub Actions** as the primary teaching tool (it's free, built into GitHub, and beginner-friendly), then show how the same ideas map to other tools.

## 1. The universal anatomy of a pipeline

Every CI/CD tool shares the same skeleton, only the keywords differ:

```
TRIGGER  →  when does it run?
JOBS     →  what work groups run?
  STEPS  →  the individual commands in a job
RUNNER   →  what machine runs it?
```

Keyword translation table (same concept, different names):

| Concept       | GitHub Actions | GitLab CI      | Azure DevOps   |
|---------------|----------------|----------------|----------------|
| Trigger       | `on`           | `rules`/`only` | `trigger`      |
| Job group     | `jobs`         | `stages`       | `stages`/`jobs`|
| Runner        | `runs-on`      | `tags`/image   | `pool`         |
| Step          | `steps`        | `script`       | `steps`        |
| Reusable step | `uses`         | `include`      | `template`     |

Learn one deeply and the others become "translation" work.

## 2. Your first pipeline (line-by-line)

File location: `.github/workflows/ci.yml` (the folder path is required by GitHub).

```yaml
name: CI                          # display name of the pipeline

on:                               # TRIGGER: when to run
  push:
    branches: [ main ]            # run on push to main
  pull_request:
    branches: [ main ]            # run on PRs targeting main

jobs:                             # JOBS: the work
  build:                          # job id (you name it)
    runs-on: ubuntu-latest        # RUNNER: a fresh Linux VM

    steps:                        # STEPS: run in order
      - name: Checkout code       # step 1: pull your repo onto the runner
        uses: actions/checkout@v4 # "uses" = a prebuilt action

      - name: Say hello           # step 2: run a shell command
        run: echo "Pipeline is running!"
```

**What happens:** you push to `main` → GitHub spins up a Linux VM → checks out your code → prints the message. That's a working (if trivial) pipeline.

`uses` vs `run`:
- `uses:` runs a **prebuilt, shareable action** (someone else wrote it).
- `run:` runs a **raw shell command** you type yourself.

## 3. A real .NET pipeline

Building on the skeleton, here's a practical CI pipeline:

```yaml
name: .NET CI

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:                          # "with" passes inputs to an action
          dotnet-version: '8.0.x'

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test
        run: dotnet test --no-build --configuration Release
```

New keyword: **`with`** supplies parameters to an action, like arguments to a function.

## 4. Variables and expressions

Pipelines need dynamic values. In GitHub Actions:

```yaml
env:                                    # define variables
  BUILD_CONFIG: Release
  DOTNET_VERSION: '8.0.x'

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - name: Use a variable
        run: dotnet build --configuration ${{ env.BUILD_CONFIG }}
```

- `${{ ... }}` is **expression syntax** — GitHub evaluates what's inside.
- Built-in contexts you'll use constantly:
  - `${{ github.sha }}` — the commit hash
  - `${{ github.ref }}` — the branch/tag ref
  - `${{ github.actor }}` — who triggered it
  - `${{ secrets.MY_SECRET }}` — a stored secret

## 5. Secrets (never hardcode passwords)

Store sensitive values in **GitHub → repo Settings → Secrets and variables → Actions**. Then reference them:

```yaml
      - name: Login to registry
        run: echo "${{ secrets.REGISTRY_PASSWORD }}" | docker login -u "${{ secrets.REGISTRY_USER }}" --password-stdin
```

**Rule:** secrets never appear in your YAML or code — only their *names* do. The values live encrypted in the platform. This is non-negotiable in real work.

## 6. Job dependencies and conditions

Jobs run in **parallel** by default. Use `needs` to chain them, and `if` to gate them.

```yaml
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - run: echo "building"

  test:
    needs: build                        # waits for build to succeed
    runs-on: ubuntu-latest
    steps:
      - run: echo "testing"

  deploy:
    needs: test                         # waits for test
    if: github.ref == 'refs/heads/main' # only deploy from main branch
    runs-on: ubuntu-latest
    steps:
      - run: echo "deploying"
```

This creates the classic flow: **build → test → deploy**, where deploy only happens on `main`.

## 7. Caching (making pipelines fast)

Re-downloading dependencies every run is slow. Cache them:

```yaml
      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: nuget-${{ hashFiles('**/*.csproj') }}
```

- `key` — if the hash of your `.csproj` files hasn't changed, reuse the cache.
- This can cut minutes off each run. A senior-level habit.

## 8. Artifacts (passing outputs between jobs / saving results)

```yaml
      - name: Upload build output
        uses: actions/upload-artifact@v4
        with:
          name: app-build
          path: ./publish

  # In a later job:
      - name: Download build output
        uses: actions/download-artifact@v4
        with:
          name: app-build
```

Artifacts persist your build so a later job (or you, via the UI) can use it. Think "boxing up the finished dish to hand to the next station."

## 9. Matrix builds (test across versions at once)

```yaml
jobs:
  test:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        dotnet: [ '6.0.x', '7.0.x', '8.0.x' ]
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ matrix.dotnet }}
      - run: dotnet test
```

This runs the job **three times in parallel**, once per .NET version. `${{ matrix.dotnet }}` takes each value in turn.

## 10. Environments and manual approval (the CD in CI/CD)

```yaml
  deploy-prod:
    needs: build-and-test
    runs-on: ubuntu-latest
    environment:
      name: production          # GitHub environment with protection rules
      url: https://myapp.com
    steps:
      - run: ./deploy.sh
```

In the GitHub UI you attach a **required reviewer** to the `production` environment. The pipeline then **pauses and waits for a human to click Approve** before deploying. That's the difference between Continuous *Delivery* (manual gate) and Continuous *Deployment* (fully automatic).

## 11. A complete intermediate pipeline (everything together)

```yaml
name: .NET CI/CD

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

env:
  BUILD_CONFIG: Release
  DOTNET_VERSION: '8.0.x'

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Cache NuGet
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: nuget-${{ hashFiles('**/*.csproj') }}

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration ${{ env.BUILD_CONFIG }}

      - name: Test
        run: dotnet test --no-build --configuration ${{ env.BUILD_CONFIG }} --logger trx

      - name: Publish
        run: dotnet publish -c ${{ env.BUILD_CONFIG }} -o ./publish

      - name: Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: app-build
          path: ./publish

  deploy-staging:
    needs: build-and-test
    if: github.ref == 'refs/heads/develop'
    runs-on: ubuntu-latest
    environment:
      name: staging
    steps:
      - name: Download artifact
        uses: actions/download-artifact@v4
        with:
          name: app-build
      - name: Deploy to staging
        run: echo "Deploying to staging..."   # replace with real deploy

  deploy-production:
    needs: build-and-test
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    environment:
      name: production                         # add a required reviewer in UI
    steps:
      - name: Download artifact
        uses: actions/download-artifact@v4
        with:
          name: app-build
      - name: Deploy to production
        run: echo "Deploying to production..." # replace with real deploy
```

Read this top to bottom — you now understand every line. That's an intermediate-level pipeline.

---

# PART C — Other Tools (quick translation)

Once GitHub Actions clicks, here's the same "build + test" idea in the two other tools you'll likely meet in a .NET shop.

## GitLab CI (`.gitlab-ci.yml`)

```yaml
stages:
  - build
  - test

build-job:
  stage: build
  image: mcr.microsoft.com/dotnet/sdk:8.0
  script:
    - dotnet build --configuration Release

test-job:
  stage: test
  image: mcr.microsoft.com/dotnet/sdk:8.0
  script:
    - dotnet test
```

Note: no `jobs:` wrapper; each top-level key is a job, and `stage:` groups them.

## Azure DevOps (`azure-pipelines.yml`)

```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
  - task: UseDotNet@2
    inputs:
      version: '8.0.x'
  - script: dotnet build --configuration Release
    displayName: 'Build'
  - script: dotnet test
    displayName: 'Test'
```

Note: `pool` = runner, `task` = prebuilt step (like `uses`), `script` = shell command.

The **concepts are identical** — trigger, runner, steps, prebuilt tasks. Only vocabulary changes.

---

# PART D — Learning Path & Practice

Do these in order. Each is small and builds confidence.

1. **Validate YAML.** Write a plain YAML file with maps, lists, and nesting. Paste into an online YAML linter until it's error-free. Goal: internalize indentation.
2. **Hello pipeline.** In a personal GitHub repo, add `.github/workflows/ci.yml` with just checkout + `echo`. Push. Watch it run in the Actions tab.
3. **Add build + test.** Point it at a small .NET project. Make a test fail on purpose — watch the pipeline go red.
4. **Add caching.** Add the NuGet cache step. Compare run times before/after.
5. **Add variables + a matrix.** Test across two .NET versions.
6. **Add artifacts.** Publish and upload the build output; download it in a second job.
7. **Add deployment gates.** Create `staging` and `production` environments, add a required reviewer to production, and watch the pipeline pause for approval.
8. **Translate.** Rewrite your final pipeline in GitLab CI or Azure DevOps syntax to prove the concepts transfer.

## Common beginner mistakes to avoid

- Using **tabs** instead of spaces (breaks everything silently-ish).
- Forgetting the **space after a colon**.
- Wrong **indent level** — a step ends up outside its job.
- Hardcoding **secrets** in the YAML.
- Missing `needs:` — jobs run in parallel when you expected them sequential.
- Wrong **file path** — GitHub only reads workflows from `.github/workflows/`.

## Mental model to keep

> YAML describes **what** you want. The CI/CD tool decides **how** to do it. Your job is to describe the assembly line clearly: when it starts (trigger), what machine runs it (runner), and the ordered steps (build → test → deploy), with gates so nothing broken reaches users.

Master this file's Part B section 11 (the complete pipeline) and you're solidly intermediate.
