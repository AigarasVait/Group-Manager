export interface TransactionDto {
    id: number;
    description: string;
    date: string;
    amount: number;
    payerId: number | null;
    groupId: number | null;
}

export interface TransactionCreateDto {
    groupId: number | null;
    payerId: number | null;
    amount: number;
    description: string;
    // 0 - equal, 1 - dynamic, 2 - percentage
    sType: 0 | 1 | 2 | null;
    splitValues?: number[];
}