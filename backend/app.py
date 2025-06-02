from flask import Flask, request, jsonify

app = Flask(__name__)

# In-memory store for TODO items
todos = []
next_todo_id = 1

@app.route('/todos', methods=['GET'])
def get_todos():
    return jsonify(todos)

@app.route('/todos', methods=['POST'])
def add_todo():
    global next_todo_id
    data = request.get_json()
    if not data or 'task' not in data:
        return jsonify({'error': 'Task is required'}), 400

    new_todo = {
        'id': next_todo_id,
        'task': data['task'],
        'completed': False
    }
    todos.append(new_todo)
    next_todo_id += 1
    return jsonify(new_todo), 201

@app.route('/todos/<int:todo_id>', methods=['PUT'])
def update_todo(todo_id):
    data = request.get_json()
    if not data or 'completed' not in data or not isinstance(data['completed'], bool):
        return jsonify({'error': 'Boolean 'completed' status is required'}), 400

    todo_to_update = None
    for todo in todos:
        if todo['id'] == todo_id:
            todo_to_update = todo
            break

    if not todo_to_update:
        return jsonify({'error': 'Todo not found'}), 404

    todo_to_update['completed'] = data['completed']
    return jsonify(todo_to_update)

if __name__ == '__main__':
    app.run(debug=True)
