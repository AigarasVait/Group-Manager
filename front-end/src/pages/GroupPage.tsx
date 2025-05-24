import { useEffect, useState } from "react";
import { fetchGroups } from "../api/groupAPI.ts";
import type { Group, GroupPost } from "../types/Group.ts";
import NewGroupForm from "../components/newGroupForm/NewGroupForm.tsx";
import { postGroup } from "../api/groupAPI.ts";
import "./GroupPage.css";

export function GroupList() {
  const [groups, setGroups] = useState<Group[]>([]);
  const [open, setOpen] = useState(false);

  useEffect(() => {
    fetchGroups().then((data) => { setGroups(data); })
  }, []);

  function handleCreate(newGroupPost: GroupPost) {
    postGroup(newGroupPost)
      .then((newGroup) => {
        setGroups((prevGroups) => [...prevGroups, newGroup])
      })
      .catch((error) => {
        console.error("Failed to add group:", error);
      });
    setOpen(false);
  }

  return (
    <div className="groupContainer">
      <div className="list-group-item d-flex justify-content-between pb-3 mx-3">
        <p className="h3">Groups</p>
        <button
          onClick={(() => setOpen(true))}
          className="btn btn-primary">
          Add new
        </button>
      </div>

      <ul className="list-group">
        {groups.map((group) => (
          <li key={group.id} className="list-group-item d-flex justify-content-between">
            <span className="fs-5">{group.name}</span>
            <button className="btn btn-secondary">View</button>
          </li>
        ))}
      </ul>

      {open &&
        (
          <>
            <div className="dark-overlay" />
            <NewGroupForm onCreate={handleCreate} onCancel={() => setOpen(false)} />
          </>
        )}

    </div>
  );
}