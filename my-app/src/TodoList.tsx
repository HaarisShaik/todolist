import axios from "axios"
import { useState } from "react"

export const TodoList = () => {
    const [todoItems, setTodoItems] = useState<TodoistItem[]>([]);
    const [todoInput, setTodoInput] = useState<string>('');
    const [hasLoadedOnce, setHasLoadedOnce] = useState<boolean>(false);

    const getTodoList = () => axios.get<TodoistItem[]>("https://localhost:7044/todo").then(r => {
            setTodoItems(r.data.map(t => ({text: t.text, id: t.id})))
        })
        .catch(e => {
            console.log(e)
        });

    const addTodo = () => axios.post('https://localhost:7044/todo', { text: todoInput })
    .then(x => {
        setTodoInput('');
        getTodoList();
    })
    .catch(e => console.log(e));

    const completeTodoListItem = (id: number) => axios.delete('https://localhost:7044/todo', {data: {id}}).then(r => getTodoList()).catch(e => console.log(e));


    if (!hasLoadedOnce) {
        getTodoList();
        setHasLoadedOnce(true);
    }

    return (
        <div>
        <h1>Todo List</h1>
        <input value={todoInput} onChange={e => setTodoInput(e.target.value)} />
        <button onClick={() => addTodo()}>Add</button>
        {todoItems.map(t => <TodoListItem text={t.text}  completeItem={() => completeTodoListItem(t.id)}/>)}
        </div>
    )
}

export interface TodoistItem {
    id: number;
    text: string;
}


interface TodoListItemProps {
    text: string
    completeItem: () => Promise<void>;
}
export const TodoListItem = (props: TodoListItemProps) => {

    return (
    <div style={{ display: 'flex', flexDirection: 'row'}}>
        <button onClick={() => props.completeItem()}>Complete</button>
        <p>{props.text}</p>
    </div>
    )
}