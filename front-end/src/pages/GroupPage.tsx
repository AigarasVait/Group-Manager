import React, { useEffect, useState } from "react";
import { fetchGroups } from "../api/groupAPI.ts";
import "./GroupPage.css";
import type {Group}  from "../types/Group.ts";

export function GroupList() {
  const [groups, setGroups] = useState<Group[]>([]);

  useEffect(() => {
    fetchGroups().then((data) => {setGroups(data);})
  }, []);

  return (
    <div className="groupContainer">
      <div className="list-group-item d-flex justify-content-between pb-3 mx-3">
        <p className="h3">Groups</p>
        <button className="btn btn-primary">Add new</button>
      </div>

      <ul className="list-group">
        {groups.map((group) => (
          <li key={group.id} className="list-group-item d-flex justify-content-between">
            <span className="fs-5">{group.name}</span>
            <button className="btn btn-secondary">View</button>
          </li>
        ))}
      </ul>
    </div>
  );
}