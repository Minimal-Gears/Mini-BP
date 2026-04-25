# Mini-BP

Mini-BP is a lightweight, code-first workflow engine for .NET applications. It focuses on sequential process execution, a task inbox (cartable), and a fluent API for defining workflows in C#.

## Features

- Code-first workflow definition with `WorkflowBuilder`
- Sequential execution for start, service, gateway, user, and end steps
- In-memory case runtime with duties, comments, and execution tracks
- Assignment rules for claiming, fixed users, conditional routing, and cyclic assignment
- Minimal sample API for starting cases and working a cartable

## Technology

- C#
- .NET 10
- Minimal API sample host

## Use Cases

Mini-BP is suitable for:
- Embedding workflow automation into existing .NET applications
- Building lightweight approval and routing processes
- Prototyping BPMS concepts without a full enterprise stack

## Project Structure

- `src/MiniBP/MiniBP.BPMS`: Core workflow abstractions, builder, runtime, and services
- `src/MiniBP/MiniBP.Infrastructure`: Dependency injection registration for the in-memory runtime
- `src/MiniBP/MiniBP.Samples.Api`: Sample API and example `LoanApplication` workflow

## Sample Workflow

The sample host registers a `LoanApplication` workflow:

- `SubmitApplication` creates a claimable duty
- `InitialAssessment` computes whether manager approval is required
- `AmountGateway` routes the case
- `ManagerApproval` creates a cyclically assigned duty
- `Archive` and `Rejected` end the case
