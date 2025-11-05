import { useEffect, useState } from "react";
import { API } from "../../api/endpoints";

export default function Courses() {
  const [courses, setCourses] = useState<string[]>([]);

  useEffect(() => {
    fetch(API.COURSES + "/list")
      .then((res) => res.json())
      .then((data) => setCourses(data))
      .catch((err) => console.error(err));
  }, []);

  return (
    <div>
      <h1>Courses</h1>
      <ul>
        {courses.map((c, i) => (
          <li key={i}>{c}</li>
        ))}
      </ul>
    </div>
  );
}
