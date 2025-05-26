export interface UserDto {
  id: number;
  name: string;
  balance?: number;
}

export interface UserPatchDto {
  addMemberUserId?: number;
  removeMemberUserId?: number;
}