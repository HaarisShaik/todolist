// window on
window.onload = function () {
  fetch('http://localhost:5000/todo') // connect to backend
  .then(response => response.json()) // Converts the response to json
  .then(data =>{
    data.forEach(todo =>{
      // data
      const li = document.createElement("li");
        li.textContent = todo.text;

        li.addEventListener("click", () => {
          li.classList.toggle("completed");
        });

        const deleteBtn = document.createElement("button");
        deleteBtn.textContent = "✖";
        deleteBtn.style.marginLeft = "10px";

       // deleteBtn.onclick = () => li.remove(); // We'll improve this later
       deleteBtn.onclick = () => {
       fetch('http://localhost:5000/todo', {
          method: 'DELETE',
          headers: {
            'Content-Type': 'application/json'
                    },
             body: JSON.stringify({ id: todo.id })
              })
            .then(response => {
             if (!response.ok) throw new Error("Failed to delete task");
             li.remove();
               })
  .catch(error => console.error("Delete failed:", error));
};

        li.appendChild(deleteBtn);
        document.getElementById("taskList").appendChild(li);
      });
    })
    .catch(error => console.error("Error loading todos:", error));
};



function addTask() {
  const taskInput = document.getElementById("taskInput");
  const taskText = taskInput.value.trim();

  if (taskText === "") return;
  
  // Send the new task to the backend
fetch('http://localhost:5000/todo', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({ text: taskText }) // Send the task as JSON
})
.then(response => {
  const li = document.createElement("li");
  li.textContent = taskText;

  li.addEventListener("click", () => {
    li.classList.toggle("completed");
  });

  const deleteBtn = document.createElement("button");
  deleteBtn.textContent = "✖";
  deleteBtn.style.marginLeft = "10px";
  deleteBtn.onclick = () => li.remove();

  li.appendChild(deleteBtn);
  document.getElementById("taskList").appendChild(li);

  taskInput.value = "";
  if (!response.ok) {
    throw new Error("Failed to save task");
  }
})
.catch(error => {
  console.error("Error saving task:", error);
});


  
}

