# 🎅 Santa Delivery – Technical Test

A small elf has accidentally broken several parts of Santa’s delivery system, causing some endpoints to behave incorrectly.  
Your mission is to **fix the broken endpoints**, **complete the missing functionality**, and **extend the system with a new feature**.

The project already contains:
- A working API structure
- Already have the basic entities and relationships defined with some data
- Partially implemented endpoints
- Some tests that may be working and other one need to be created


---

## 🧩 Domain Overview

Santa delivers gifts to children around the world.

The system manages:
- Children and their wishlists
- Delivery routes
- Scheduled deliveries
- Naughty and nice children
- (To be added) Reindeers assigned to routes

---

## 📌 Part 1 – Fix Existing Endpoints

Several existing endpoints are not behaving correctly due to the elf’s mistake.

### Santa needs to:

- Retrieve the **list of naughty children**, so he knows how much coal he needs to bring.
- Retrieve the **list of failed deliveries**.
- Retrieve the **wishlist of a specific child**.
- Retrieve the **wishlist ordered by priority** (highest priority first).

Some of these endpoints may:
- Return incorrect data
- Miss filters
- May need to be created from scratch
- Missing validations

You are expected to:
- Identify and fix the issues
- Improve or add tests where necessary
- Ensure the API behaves consistently and predictably

---

## 📌 Part 2 – Extend the System

Santa also needs better control over his logistics.

Currently, the system does **not** store information about **reindeers assigned to delivery routes**.  
Santa needs this information to prevent routes from becoming overloaded or running out of reindeers.

### New Entity: Reindeer

Each reindeer can perform **multiple deliveries**.

The new entity must contain at least:

- `Identifier`
- `Name`
- `PlateNumber`
- `Weight`
- `Packets`

You must:
- Create the new entity
- Create the required endpoint(s) to manage it
-- Ensure reindeers can be assigned to delivery routes
- Add tests covering the new functionality

---

## 🧪 Part 3 – Testing Requirements

This repository includes a test suite that describes the **expected behavior** of the API.

At the start, **several tests will fail (red)** because a small elf has broken parts of the implementation.  
The tests themselves are **correct**: your task is to **fix the code** until all tests pass.

### Rules
- ✅ You may add new tests if you want to cover additional edge cases.
- 🚫 Do not modify the existing tests (unless there is an obvious typo and you explain the change).
- ✅ The goal is that the existing suite becomes green by fixing the implementation.


---
## 🧪 Extra – Structure solid and Readability

Help santa by ensuring the code is clean, readable, and well-structured.

- ✅ Refactor any parts of the code that you find hard to read or poorly structured.
  - The code should follow common good practices (such as separation of concerns and SOLID principles).
  - Focus on clean code and **separation of responsibilities**.

- ✅ Ensure that the code follows best practices and coding standards.

---

## ✅ What We Are Evaluating

We will evaluate:

- Correctness of the implemented logic
- Data filtering and validation
- Code readability and structure
- Quality and clarity of tests
- Ability to work with existing (imperfect) code

---

## ⏱️ Notes

- You do not need to build a UI.
- Focus on backend logic and tests.
- You are free to refactor existing code if needed.

Good luck, and help Santa save Christmas! 🎄🎁
