def simulate_guard(map, obstruction=None):
    visited = set()
    position = guard_start
    direction = guard_direction
    while position is within map bounds:
        if obstruction and position == obstruction:
            # Treat the obstruction as an impassable cell
            direction = turn_right(direction)
        elif map[position_in_front(position, direction)] == '#':
            direction = turn_right(direction)
        else:
            position = step_forward(position, direction)
        state = (position, direction)
        if state in visited:
            return True  # Loop detected
        visited.add(state)
    return False  # No loop

def find_possible_obstructions(map, guard_start, guard_direction):
    valid_positions = set()
    for position in all_empty_cells(map):
        if position == guard_start:
            continue
        if simulate_guard(map, obstruction=position):
            valid_positions.add(position)
    return valid_positions

# Read input and initialize map, guard_start, and guard_direction
map = read_map()
guard_start = find_guard_start(map)
guard_direction = find_guard_direction(map)

# Find all valid obstruction positions
valid_positions = find_possible_obstructions(map, guard_start, guard_direction)
print(len(valid_positions))
