import type { User } from './User';
import type { TransactionDto } from './Transaction';
import type { DebtTracker } from './DebtTracker';

export interface GroupDto {
  id: number;
  name: string;
  members: User[];
  transactions: TransactionDto[];
  debtTracker: DebtTracker[];
}

export interface GroupSimpleDto {
  id: number | null;
  name: string;
  creatorId: number | null;
  balance: number;
}