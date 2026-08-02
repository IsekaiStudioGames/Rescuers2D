# DeverQuest 0.28.0 — Compensation Preview

Milestone 28 adds an intentionally limited planning calculator for Guilds that
want to compare finalized DeverQuest time with an optional compensation rate.
It does not turn DeverQuest into payroll software.

## Configure a Policy

Authenticate as a Boss or CEO, then open:

**Tools > DeverQuest > Workspaces > Guild Hall**

Expand **Guild Accounts and Authority > Compensation Preview Policies** and
select an Adventurer account.

Choose:

- **Enable Preview** — disabled for every migrated and new account by default.
- **Basis: Hourly** — multiplies eligible finalized hours by the configured
  hourly rate.
- **Basis: Annual Salary** — converts annual salary to a tracking equivalent
  using `annual salary / (52 × scheduled weekly hours)`, then multiplies that
  equivalent by eligible finalized hours.
- **Currency Code** — a three-letter display code. Invalid values normalize to
  `USD`.
- **Include Approved Breaks** — includes completed Approved Break seconds.
  Meditation and Idle/Unverified time remain excluded.
- **Chronicle Eligibility** — includes only sealed/verified Chronicles, or
  allows legacy/unsealed Chronicles too.

Select **Save Preview Policy**. The authority audit records that a policy
changed but deliberately omits the rate.

## Review an Estimate

The authenticated Adventurer opens:

**Tools > DeverQuest > Workspaces > Rewards and History**

Expand **Compensation Preview**. It shows:

- current Monday-through-today eligible time and estimate;
- eligible time and estimate for the active History filters;
- the number of included Chronicles and Quests;
- modified/unavailable time excluded from the estimate;
- legacy time excluded by a verified-only policy; and
- included time matching configured suspicious-session or suspicious-frequency
  flags, so an administrator can review it.

The optional CSV export writes a separate planning statement. It does not
alter, reseal, or append to any Chronicle.

## What Counts

| Classification | Eligible? |
| --- | --- |
| Finalized Focused Work | Yes, subject to Chronicle policy |
| Completed Approved Break | Only when the account policy enables it |
| Meditation | Never |
| Idle/Unverified | Never |
| Active, unfinished Quest | Never |
| Modified/unavailable Chronicle | Never |
| Legacy/unsealed Chronicle | Only when explicitly allowed |

Quest flags do not silently delete otherwise eligible time. They are shown as
manual-review warnings because the calculator cannot decide whether unusual
work was authorized.

## Legal and Privacy Boundary

Every preview and export is labeled with this boundary:

> Planning estimate only. This is not payroll, a wage statement, a promise of
> payment, tax advice, or authorization to pay. A Guild administrator must
> review approved time and apply the actual employment agreement and applicable
> law.

DeverQuest does not calculate taxes, withholding, overtime, benefits,
contractor status, minimum wage, pay periods, direct deposits, or payments.

Rates are stored with local Unity editor preferences for the Guild account.
They are not written to daily timecards, published shared-Guild snapshots, or
included in authority-audit details. Local preference storage is not encrypted
or suitable as an authoritative payroll database.

## Performance

The panel calculates only while **Rewards & History** is visible and operates
on the History service's existing cached records. It does not scan the
AssetDatabase, add a background payroll loop, or add work to the live timer's
repaint path.

## Unity Import Checklist

1. Install `com.echodevgames.deverquest-0.28.0.tgz` through Unity Package
   Manager and confirm the Console has no compiler errors.
2. Log in as a Boss or CEO and confirm every migrated account starts with
   Compensation Preview disabled.
3. Configure an hourly policy, save it, then reopen the foldout and confirm the
   values persisted.
4. Open Rewards & History and verify that a one-hour eligible sample at
   `USD 20.00` displays `USD 20.00`.
5. Switch to Annual Salary, use `USD 52,000` and `40` scheduled hours/week,
   and confirm the effective equivalent is `USD 25.00` per hour.
6. Confirm Meditation and Idle/Unverified seconds do not change eligible time.
7. Toggle Approved Break inclusion and confirm only Approved Break seconds are
   added.
8. Under Verified-only eligibility, confirm a legacy Chronicle is reported as
   excluded. Under Verified-plus-Legacy, confirm it becomes eligible.
9. Deliberately copy and edit a test Chronicle, refresh History, and confirm
   the modified Chronicle is excluded. Restore the test backup afterward.
10. Export the filtered CSV and confirm it contains the planning disclaimer.
11. Inspect a daily Markdown timecard and shared Guild snapshot to confirm no
    compensation rate or estimate was added.
12. Keep the live Quest workspace open for several minutes and confirm the new
    feature did not add timer-repaint or AssetDatabase slowdown.
