document.addEventListener('DOMContentLoaded', () => {
    const todoList = document.getElementById('todo-list');
    const newTaskInput = document.getElementById('new-task-input');
    const addTaskBtn = document.getElementById('add-task-btn');

    const API_BASE_URL = '/todos'; // Assuming backend is on the same host

    async function fetchTodos() {
        try {
            const response = await fetch(API_BASE_URL);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const todos = await response.json();
            renderTodos(todos);
        } catch (error) {
            console.error('Error fetching todos:', error);
            todoList.innerHTML = '<li>Error loading todos. Please check if the backend is running.</li>';
        }
    }

    function renderTodos(todos) {
        todoList.innerHTML = ''; // Clear existing todos
        if (todos.length === 0) {
            todoList.innerHTML = '<li>No tasks yet! Add one above.</li>';
            return;
        }
        todos.forEach(todo => {
            const listItem = document.createElement('li');
            listItem.textContent = todo.task;
            if (todo.completed) {
                listItem.classList.add('completed');
            }

            // Create a button to toggle completion status
            const completeButton = document.createElement('button');
            completeButton.textContent = todo.completed ? 'Undo' : 'Complete';
            completeButton.style.marginLeft = '10px'; // Add some spacing

            completeButton.addEventListener('click', async () => {
                await toggleTodoComplete(todo.id, !todo.completed);
            });

            listItem.appendChild(completeButton);
            todoList.appendChild(listItem);
        });
    }

    async function addTask() {
        const taskText = newTaskInput.value.trim();
        if (!taskText) {
            alert('Please enter a task!');
            return;
        }

        try {
            const response = await fetch(API_BASE_URL, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ task: taskText }),
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            newTaskInput.value = ''; // Clear input field
            fetchTodos(); // Refetch and re-render all todos
        } catch (error) {
            console.error('Error adding todo:', error);
            alert('Failed to add task. Please try again.');
        }
    }

    async function toggleTodoComplete(todoId, newCompletedStatus) {
        try {
            const response = await fetch(`${API_BASE_URL}/${todoId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ completed: newCompletedStatus }),
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(`HTTP error! status: ${response.status}, message: ${errorData.error || 'Unknown error'}`);
            }
            fetchTodos(); // Refetch and re-render all todos to reflect the change
        } catch (error) {
            console.error('Error updating todo status:', error);
            alert(`Failed to update task status: ${error.message}`);
        }
    }

    addTaskBtn.addEventListener('click', addTask);
    newTaskInput.addEventListener('keypress', (event) => {
        if (event.key === 'Enter') {
            addTask();
        }
    });

    // Initial fetch
    fetchTodos();
});
