import React, { useEffect, useState } from "react";
import { apiFetch } from "../api/apiFetch";
import "./GroupPage.css";

interface group {
  id: number;
  name: string;
  balance: number;
}

export function GroupList() {
  const [groups, setGroups] = useState<group[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    apiFetch("/groups")   // call your backend API endpoint
      .then((res) => res.json())
      .then((data) => {
        setGroups(data);
        setLoading(false);
      })
      .catch((error) => {
        console.error("Failed to fetch groups", error);
        setLoading(false);
      });
  }, []);

  if (loading) {
    return React.createElement("div", { style: { color: "black" } }, "Loading...");
  }

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