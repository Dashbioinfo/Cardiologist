function [Factor]=fitting(Signal,Target);
z1=size(Signal);
z2=size(Target);
if z1==z2
Factor=((Signal'*Signal).^-1)*Signal'*Target;
else
Target=Target';
Factor=((Signal'*Signal).^-1)*Signal'*Target;
end
Factor=Factor./((length(Signal)).^2);
end
