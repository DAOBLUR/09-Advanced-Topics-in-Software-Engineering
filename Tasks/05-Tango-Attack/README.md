## The David-Prasad Protocol
- **Step 1**

**Reader** --request--> **Server**

**Reader** <--response(certificate C)-- **Server**

- **Step 2**

**Reader** --request(ID)--> **Tag**

**Reader** <--response(PID2)-- **Tag**

- **Step 3**

**Reader** --request(Tuple(PID, C))--> **Server**

**Reader** <--response(K1, K2)-- **Server** or **

## Traceability Attack
A = Adversary(Tag T, Reader R).

Execute(R, T, Session i) -> Listener.

Test(Session i, T0, T1) -> Generate bit 'b' and PID2

- **Phase 1 (Learning)**: A use Execute many times.
- **Phase 2 (Challenge)**:  A use Test.
- **Phase 3 (Guessing)**: Generate d bit as its conjecture of the value of b.

t = ID bit length.

r = nº times A runs Execute.

Adv(t, r) = |Pr[d = b] - 1/2|
