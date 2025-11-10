import type { FC } from "react";

interface Course {
	id: string;
	title: string;
	description?: string;
	author?: string;
	category?: string;
}

const cardStyle: React.CSSProperties = {
	border: "1px solid #e0e0e0",
	borderRadius: 8,
	padding: "1rem",
	background: "white",
	boxShadow: "0 1px 3px rgba(0,0,0,0.06)",
};

const CourseCard: FC<{ course: Course }> = ({ course }) => {
	return (
		<article style={cardStyle} aria-labelledby={`course-${course.id}`}>
			<h3 id={`course-${course.id}`} style={{ margin: "0 0 .5rem 0" }}>
				{course.title}
			</h3>
			{course.category && (
				<div style={{ fontSize: ".8rem", color: "#666", marginBottom: ".5rem" }}>
					{course.category}
				</div>
			)}
			<p style={{ margin: "0 0 .75rem 0", color: "#333" }}>{course.description}</p>
			<div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
				<small style={{ color: "#666" }}>{course.author ?? "Unknown"}</small>
				<button style={{ padding: ".4rem .6rem", borderRadius: 4 }}>View</button>
			</div>
		</article>
	);
};

export default CourseCard;
