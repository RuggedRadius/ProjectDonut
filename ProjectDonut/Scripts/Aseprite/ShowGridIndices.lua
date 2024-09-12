-- Create a new layer for indices
local sprite = app.activeSprite
local indexLayer = sprite:newLayer()
indexLayer.name = "Grid Indices"

-- Define font and size
local font = "Arial" -- Change as needed
local fontSize = 8    -- Change as needed

-- Loop through each grid cell
for y = 0, sprite.height - 1, sprite.gridHeight do
  for x = 0, sprite.width - 1, sprite.gridWidth do
    -- Draw index in each cell
    local index = (y / sprite.gridHeight) * (sprite.width / sprite.gridWidth) + (x / sprite.gridWidth) + 1
    app.activeImage:drawText(tostring(index), x, y, font, fontSize, Color{255, 255, 255})
  end
end
